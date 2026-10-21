namespace DocumentAnalysisInterface.Startup;

/// <summary>One stage of the app's startup (see <see cref="AppStartup"/>), and how far it's got.</summary>
/// <param name="Done">For a stage that counts its work: how much is done - else 0.</param>
/// <param name="Total">For a stage that counts its work: how much there is - else 0, meaning the stage runs for as long as it takes.</param>
public sealed record StartupStage(string Name, StartupStageState State, int Done = 0, int Total = 0, TimeSpan? Took = null);

public enum StartupStageState { Pending, Running, Finished }

/// <summary>
/// What the app does before it can serve its pages - build the grammar, read and tokenize the corpus, open the workspaces,
/// score the working grammar - as it happens, for a page to show while it does: the stages, which one's under way and
/// how far it's got, and whether it's all done or has failed. The web host starts first, so the browser sees this page
/// rather than nothing until the work is done; everything that needs the work waits on <see cref="WhenReady"/>.
/// </summary>
public sealed class AppStartup
{
    readonly object _gate = new();
    readonly List<StartupStage> _stages = [];
    readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    readonly System.Diagnostics.Stopwatch _elapsed = System.Diagnostics.Stopwatch.StartNew();
    System.Diagnostics.Stopwatch _stage;
    DateTime _lastChange;

    /// <summary>The stages, in order, as they stand.</summary>
    public IReadOnlyList<StartupStage> Stages
    {
        get
        {
            lock (_gate)
                return _stages.ToList();
        }
    }

    /// <summary>Whether everything the pages need is ready.</summary>
    public bool IsReady => _ready.Task.IsCompletedSuccessfully;

    /// <summary>Completes once everything the pages need is ready - or faults, if startup did.</summary>
    public Task WhenReady => _ready.Task;

    /// <summary>What startup failed with, if it did.</summary>
    public Exception Error { get; private set; }

    /// <summary>How long since the app started.</summary>
    public TimeSpan Elapsed => _elapsed.Elapsed;

    /// <summary>Fires, from whatever thread, when anything here changes - at most a few times a second while a stage counts its work.</summary>
    public event Action Changed;

    /// <summary>Declares the stages to come, in order - so the page can show what's ahead as well as what's done.</summary>
    public void Plan(params string[] stages)
    {
        lock (_gate)
            foreach (var stage in stages)
                _stages.Add(new(stage, StartupStageState.Pending));

        Notify(force: true);
    }

    /// <summary>Starts the named stage (declared by <see cref="Plan"/>, or added now), finishing the one under way.</summary>
    public void Begin(string stage)
    {
        lock (_gate)
        {
            FinishRunning();
            var index = _stages.FindIndex(x => x.Name == stage);

            if (index < 0)
            {
                _stages.Add(new(stage, StartupStageState.Running));
            }
            else
                _stages[index] = _stages[index] with { State = StartupStageState.Running };

            _stage = System.Diagnostics.Stopwatch.StartNew();
        }

        Notify(force: true);
    }

    /// <summary>How far the stage under way has got, where it counts its work.</summary>
    public void Progress(int done, int total)
    {
        lock (_gate)
        {
            var index = _stages.FindIndex(x => x.State == StartupStageState.Running);

            if (index < 0)
                return;

            _stages[index] = _stages[index] with { Done = done, Total = total };
        }

        Notify(force: done == total);
    }

    /// <summary>Everything's ready: the stage under way is finished, and whoever waits on <see cref="WhenReady"/> goes on.</summary>
    public void Ready()
    {
        lock (_gate)
            FinishRunning();

        _ready.TrySetResult();
        Notify(force: true);
    }

    /// <summary>Startup failed: the page shows why, and whoever waits on <see cref="WhenReady"/> gets the exception.</summary>
    public void Fail(Exception exception)
    {
        Error = exception;
        _ready.TrySetException(exception);
        Notify(force: true);
    }

    void FinishRunning()
    {
        var index = _stages.FindIndex(x => x.State == StartupStageState.Running);

        if (index >= 0)
            _stages[index] = _stages[index] with { State = StartupStageState.Finished, Took = _stage?.Elapsed };
    }

    /// <summary>Raises <see cref="Changed"/> - at most every quarter second, unless <paramref name="force"/>d, since a counting stage reports often.</summary>
    void Notify(bool force)
    {
        var now = DateTime.UtcNow;

        lock (_gate)
        {
            if (!force && now - _lastChange < TimeSpan.FromMilliseconds(250))
                return;

            _lastChange = now;
        }

        Changed?.Invoke();
    }
}
