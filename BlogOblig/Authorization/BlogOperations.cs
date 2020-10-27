using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BlogOblig.Authorization
{
    public static class BlogOperations
    {
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };
        public static OperationAuthorizationRequirement Read =
          new OperationAuthorizationRequirement { Name = Constants.ReadOperationName };
        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement { Name = Constants.UpdateOperationName };
        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
        public static OperationAuthorizationRequirement Open =
            new OperationAuthorizationRequirement { Name = Constants.OpenOperationName };
        public static OperationAuthorizationRequirement Closed =
            new OperationAuthorizationRequirement { Name = Constants.ClosedOperationName };
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string OpenOperationName = "Open";
        public static readonly string ClosedOperationName = "Closed";

        public static readonly string BlogAdministratorsRole = "BlogAdministrators";
    }
}