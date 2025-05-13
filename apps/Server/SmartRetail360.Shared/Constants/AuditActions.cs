namespace SmartRetail360.Shared.Constants;

public static class AuditActions
{
    // ===== Tenant operations =====
    public const string RegisterTenant = "REGISTER_TENANT";
    public const string UpdateTenantInfo = "UPDATE_TENANT_INFO";
    public const string DeleteTenant = "DELETE_TENANT";

    // ===== User operations =====
    public const string UserRegister = "USER_REGISTER";
    public const string UserLoginSuccess = "USER_LOGIN_SUCCESS";
    public const string UserLoginFailed = "USER_LOGIN_FAILED";
    public const string UserLogout = "USER_LOGOUT";
    public const string UserResetPassword = "USER_RESET_PASSWORD";
    public const string UserUpdateProfile = "USER_UPDATE_PROFILE";
    public const string UserDelete = "USER_DELETE";

    // ===== Role & Permission operations =====
    public const string AssignRole = "ASSIGN_ROLE";
    public const string RemoveRole = "REMOVE_ROLE";
    public const string UpdateRolePermissions = "UPDATE_ROLE_PERMISSIONS";

    // ===== Data operations =====
    public const string UploadDataset = "UPLOAD_DATASET";
    public const string DeleteDataset = "DELETE_DATASET";
    public const string ImportOrders = "IMPORT_ORDERS";
    public const string ExportReport = "EXPORT_REPORT";
    public const string DownloadFile = "DOWNLOAD_FILE";

    // ===== ML & Recommendation =====
    public const string TrainModel = "TRAIN_MODEL";
    public const string PredictSales = "PREDICT_SALES";
    public const string GenerateRecommendation = "GENERATE_RECOMMENDATION";
    public const string ModelRetrainTriggered = "MODEL_RETRAIN_TRIGGERED";

    // ===== Chart & Report operations =====
    public const string ViewDashboard = "VIEW_DASHBOARD";
    public const string CustomizeChart = "CUSTOMIZE_CHART";
    public const string DeleteReport = "DELETE_REPORT";

    // ===== System events =====
    public const string SystemNotificationSent = "SYSTEM_NOTIFICATION_SENT";
    public const string EmailActivationSent = "EMAIL_ACTIVATION_SENT";
    public const string EmailActivationFailed = "EMAIL_ACTIVATION_FAILED";

    // ===== Copilot / AI usage =====
    public const string CopilotQuery = "COPILOT_QUERY";
    public const string CopilotFeedbackSubmitted = "COPILOT_FEEDBACK_SUBMITTED";

    // ===== Generic resource actions =====
    public const string DeleteResource = "DELETE_RESOURCE";
    public const string UpdateResource = "UPDATE_RESOURCE";
    
    // ===== General =====
    public const string RequestReceived = "REQUEST_RECEIVED";
    
    // ===== Redis =====
    public const string GenerateAccountLockFailed = "GENERATE_ACCOUNT_LOCK_FAILED";
    
    // ===== Email Sending =====
    public const string SendEmailFailed = "SEND_EMAIL_FAILED";
}