namespace Giovanni.Task1.Extensions;

public static class HttpStatusCode
{
    public static bool IsSuccessStatusCode(this System.Net.HttpStatusCode statusCode ) => (int)statusCode >= 200 && (int)statusCode <= 299;
}