using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using ApiEcommerce.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(CorsPolicyNames.AllowSpecificOrigin)]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }
            return Ok(categoriesDto);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategory(int id)
        {
            var category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound($"Category with id {id} doesn't exists");
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists");
                return BadRequest(ModelState);
            }
            var category = _mapper.Map<Category>(createCategoryDto);
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when saving the record ${category}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategory", new {id = category.Id}, category);
        }

        [HttpPatch("{id:int}", Name="UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            if (updateCategoryDto == null)
            {
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"Category with id {id} doesn't exists");
            }
            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "The category already exists");
                return BadRequest(ModelState);
            }
            var category = _mapper.Map<Category>(updateCategoryDto);
            category.Id = id;
            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when updating the record ${category}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name="DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"Category with id {id} doesn't exists");
            }
            var category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound($"Category with id {id} doesn't exists");
            }
            category.Id = id;
            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when deleting the record ${category}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}