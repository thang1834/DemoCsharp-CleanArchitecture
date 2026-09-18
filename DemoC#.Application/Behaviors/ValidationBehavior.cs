using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Application.Behaviors
{
    /// <summary>
    /// Pipeline Behavior (Màng lọc): Cấu hình MediatR để tự động chặn các request.
    /// Giống như một anh bảo vệ đứng ở cửa, mọi Request (Command/Query) từ Controller đẩy xuống 
    /// đều phải đi qua hàm Handle() này trước khi vào tới tầng logic chính.
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Kiểm tra xem Request này có được đăng ký class Validator nào không?
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // Chạy toàn bộ các rules cấu hình trong thư viện FluentValidation
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Any())
                {
                    // Ném lỗi ngay lập tức nếu dữ liệu đầu vào vi phạm quy tắc.
                    // GlobalExceptionHandler ở tầng API sẽ bắt lỗi này và trả về HTTP 400.
                    throw new ArgumentException(failures.First().ErrorMessage);
                }
            }

            // Nếu dữ liệu sạch, cho phép đi tiếp vào Handler chính.
            return await next();
        }
    }
}
