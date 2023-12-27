namespace ApiTemplate.Application.Common.Interfaces;

public interface IUtilsService : IScopedService
{
    string GenerateRandomPassword(int length = 20);

    string GetFileHash(byte[] file);

    /// <summary>
    /// Generates a thumbnail from a base64 image respecting its original ratio
    /// </summary>
    /// <returns>Resized base64 image</returns>
    string GenerateThumbnailFromBase64(string base64Image, int width, int height);

    bool CompareProperties<T1, T2>(T1 objectA, T2 objectB, IEnumerable<string> propsToCompare);

    void AssignProperties<T1, T2>(T1 sourceObject, T2 targetObject, IEnumerable<string> propsToAssign);

    void AssignNullValues<T>(T targetObject, IEnumerable<string> propsToSetNull);
}
