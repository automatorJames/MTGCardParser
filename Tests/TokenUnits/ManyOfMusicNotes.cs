namespace Tests.Glyphs;

public class ManyOfMusicNotes : Glyph
{
    public override Nib[] Nibs => ["this melody contains the notes", Prop(MusicNotes)];

    public ManyOf<MusicNote> MusicNotes { get; set; }

}
