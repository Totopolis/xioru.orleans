namespace Xioru.Grain.Contracts;

public static class GrainConstants
{
    public const string StreamProviderName = "SMSProvider";

    public const string StateStorageName = "mainStateStore";

    public const string ProjectRepositoryStreamNamespace = "ProjectRepositoryStream";

    public const string ClusterRepositoryStreamNamespace = "ClusterRepositoryStream";

    public static Guid ClusterStreamId = new Guid("1BE1582F-E71B-4763-AA0C-EA8B1D925174");
}
