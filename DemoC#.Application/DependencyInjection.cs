using DemoC_.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DemoC_.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Tự động quét và đăng ký tất cả các Validator
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Tự động quét và đăng ký MediatR (Commands/Queries)
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                // Đăng ký Pipeline Behavior cho Validation
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}
