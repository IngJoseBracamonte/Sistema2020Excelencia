namespace SistemaSatHospitalario.Core.Domain.Constants
{
    public static class PermissionConstants
    {
        public const string Type = "Permission";

        public static class Dashboard
        {
            public const string View = "Permissions.Dashboard.View";
        }

        public static class Facturacion
        {
            public const string View = "Permissions.Facturacion.View";
            public const string Create = "Permissions.Facturacion.Create";
            public const string Cancel = "Permissions.Facturacion.Cancel";
        }

        public static class Citas
        {
            public const string ViewControl = "Permissions.Citas.ViewControl";
            public const string Manage = "Permissions.Citas.Manage";
            public const string Cancel = "Permissions.Citas.Cancel";
            public const string Atender = "Permissions.Citas.Atender";
        }

        public static class Reportes
        {
            public const string ViewCxC = "Permissions.Reportes.ViewCxC";
            public const string ViewExpediente = "Permissions.Reportes.ViewExpediente";
            public const string ViewAudit = "Permissions.Reportes.ViewAudit";
            public const string ViewOrders = "Permissions.Reportes.ViewOrders";
        }

        public static class Admin
        {
            public const string AccessSettings = "Permissions.Admin.AccessSettings";
            public const string ManageUsers = "Permissions.Admin.ManageUsers";
            public const string ManageCatalog = "Permissions.Admin.ManageCatalog";
            public const string ManageMedicos = "Permissions.Admin.ManageMedicos";
            public const string ManageTasa = "Permissions.Admin.ManageTasa";
        }
    }
}
