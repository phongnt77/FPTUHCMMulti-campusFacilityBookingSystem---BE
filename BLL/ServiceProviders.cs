using BLL.Classes;
using BLL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class ServiceProviders
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Register all services here
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFacilityService, FacilityService>();
            services.AddScoped<ICampusService, CampusService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IFacilityTypeService, FacilityTypeService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IBookingFeedbackService, BookingFeedbackService>();
            services.AddScoped<IFacilityMaintenanceService, FacilityMaintenanceService>();
            services.AddScoped<IFacilityImageService, FacilityImageService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
