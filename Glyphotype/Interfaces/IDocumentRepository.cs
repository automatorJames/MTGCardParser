namespace Glyphotype.Interfaces;

public interface IDocumentRepository
{
    public Task<List<IDocument>> GetDocumentsAsync();
}