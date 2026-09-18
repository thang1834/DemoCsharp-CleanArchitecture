using DemoC_.Application.DTOs;
using DemoC_.Application.Features.Products.Commands.AddProduct;
using DemoC_.Application.Features.Products.Commands.DeleteProduct;
using DemoC_.Application.Features.Products.Commands.UpdateProduct;
using DemoC_.Application.Features.Products.Queries.GetProductById;
using DemoC_.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách toàn bộ sản phẩm trong hệ thống (Có phân trang).
        /// </summary>
        /// <remarks>
        /// API này dùng để truy xuất danh sách sản phẩm. Sử dụng cơ chế phân trang để tránh tải quá nhiều dữ liệu cùng lúc.
        /// 
        /// **Sample request:**
        /// 
        ///     GET /api/Product?pageNumber=1&amp;pageSize=10
        /// </remarks>
        /// <param name="pageNumber">Số thứ tự của trang cần lấy (Mặc định: 1).</param>
        /// <param name="pageSize">Số lượng bản ghi trên mỗi trang (Mặc định: 10).</param>
        /// <returns>Trả về một đối tượng phân trang chứa danh sách sản phẩm và tổng số lượng.</returns>
        /// <response code="200">Truy xuất danh sách sản phẩm thành công.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponse<ProductResponseDto>>> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _mediator.Send(new GetProductsQuery(pageNumber, pageSize));
            return Ok(data);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một sản phẩm dựa vào ID.
        /// </summary>
        /// <remarks>
        /// Tìm kiếm sản phẩm trong cơ sở dữ liệu dựa trên mã định danh (ID) duy nhất.
        /// 
        /// **Sample request:**
        /// 
        ///     GET /api/Product/1
        /// </remarks>
        /// <param name="id">Mã định danh (ID) của sản phẩm cần tìm.</param>
        /// <returns>Thông tin chi tiết của sản phẩm nếu tìm thấy.</returns>
        /// <response code="200">Tìm thấy và trả về thông tin sản phẩm thành công.</response>
        /// <response code="404">Không tìm thấy sản phẩm nào khớp với ID được cung cấp (Trả về ProblemDetails).</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetProductByID(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            return Ok(product);
        }

        /// <summary>
        /// Thêm mới một sản phẩm vào hệ thống.
        /// </summary>
        /// <remarks>
        /// API này nhận dữ liệu đầu vào là JSON, đưa qua bộ lọc Validation (FluentValidation) để kiểm tra tính hợp lệ trước khi gọi xuống Database.
        /// 
        /// **Sample request:**
        /// 
        ///     POST /api/Product
        ///     {
        ///        "name": "Tai nghe Sony WH-1000XM5",
        ///        "description": "Tai nghe chống ồn chủ động cao cấp.",
        ///        "price": 7500000,
        ///        "quantity": 15,
        ///        "category": "Electronics"
        ///     }
        ///     
        /// **Ghi chú:** Thuộc tính `category` (Danh mục) hiện hỗ trợ: Electronics, Clothing, Food, Others
        /// </remarks>
        /// <param name="dto">Đối tượng chứa các thông tin cần thiết để tạo mới sản phẩm (Tên, Giá, SL...).</param>
        /// <returns>Trả về thông tin chi tiết của sản phẩm vừa được tạo thành công, kèm theo ID mới.</returns>
        /// <response code="201">Đã tạo sản phẩm thành công trong cơ sở dữ liệu.</response>
        /// <response code="400">Dữ liệu đầu vào bị sai định dạng JSON hoặc không thỏa mãn Validation (giá âm, trống tên...).</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponseDto>> AddProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var createdProduct = await _mediator.Send(new AddProductCommand(dto));
            return CreatedAtAction(nameof(GetProductByID), new { id = createdProduct.Id }, createdProduct);
        }

        /// <summary>
        /// Cập nhật toàn bộ thông tin của một sản phẩm.
        /// </summary>
        /// <remarks>
        /// API này thực hiện cập nhật thông tin sản phẩm. Lưu ý: ID truyền trên URL (`id`) và ID bên trong JSON Body (`dto.Id`) bắt buộc phải trùng khớp nhau.
        /// 
        /// **Sample request:**
        /// 
        ///     PUT /api/Product/1
        ///     {
        ///        "id": 1,
        ///        "name": "Tai nghe Sony WH-1000XM5 (Đã cập nhật)",
        ///        "description": "Phiên bản màu đen mới về hàng.",
        ///        "price": 7200000,
        ///        "quantity": 20,
        ///        "category": "Electronics"
        ///     }
        /// </remarks>
        /// <param name="id">Mã định danh của sản phẩm cần cập nhật.</param>
        /// <param name="dto">Đối tượng chứa các thông tin mới của sản phẩm.</param>
        /// <returns>Không trả về dữ liệu Body (HTTP 204).</returns>
        /// <response code="204">Cập nhật sản phẩm thành công.</response>
        /// <response code="400">ID trên URL không khớp với ID trong Body, hoặc dữ liệu vi phạm Validation.</response>
        /// <response code="404">Không tìm thấy sản phẩm có ID này để cập nhật.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _mediator.Send(new UpdateProductCommand(dto));
            return NoContent();
        }

        /// <summary>
        /// Xóa vĩnh viễn một sản phẩm khỏi hệ thống.
        /// </summary>
        /// <remarks>
        /// Thực hiện tìm kiếm sản phẩm theo ID và xóa nó. Nếu không tồn tại, sẽ trả về lỗi 404.
        /// 
        /// **Sample request:**
        /// 
        ///     DELETE /api/Product/1
        /// </remarks>
        /// <param name="id">Mã định danh (ID) của sản phẩm cần xóa.</param>
        /// <returns>Không trả về dữ liệu Body (HTTP 204).</returns>
        /// <response code="204">Đã xóa sản phẩm thành công.</response>
        /// <response code="404">Không tìm thấy sản phẩm cần xóa.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _mediator.Send(new DeleteProductCommand(id));
            return NoContent();
        }
    }
}
