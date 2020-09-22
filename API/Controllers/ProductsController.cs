using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRespository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRespository<ProductBrand> _productBrandRepo;
        private readonly IGenericRespository<Product> _productRepo;
        public ProductsController(IGenericRespository<Product> productRepo,
                                  IGenericRespository<ProductBrand> productBrandRepo,
                                  IGenericRespository<ProductType> productTypeRepo, 
                                  IMapper mapper)
        {
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProductsAsync
            (string sort, int? brandId, int? typeId )
        { 
            var spec = new ProductsWithTypesAndBrandsSpecification(sort, brandId, typeId);

            var products = await _productRepo.ListAsync(spec);

            return Ok(_mapper
            .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProductAsync(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id); 

            var product = await _productRepo.GetEntityWithSpec(spec);

            if(product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<ProductBrand>>> GetProductBrandsAsync()
        {
            var brands = await _productBrandRepo.ListAllAsync();

            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<ProductType>>> GetProductTypesAsync()
        {
            var types = await _productTypeRepo.ListAllAsync();

            return Ok(types);
        }
    }

}