namespace Tests.TokenUnits;

public class ManyOfMusicNotes : TokenUnit
{
    protected override Snippet[] Snippets => ["this melody contains the notes", Prop(MusicNotes)];

    public ManyOf<MusicNote> MusicNotes { get; set; }

}
