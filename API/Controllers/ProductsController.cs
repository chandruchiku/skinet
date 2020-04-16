using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
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
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProductsAsync()
        { 
            // Video 39 - I have made modification not to create the class
            var spec = new BaseSpecification<Product>();
            spec.AddInclude(x => x.ProductType);
            spec.AddInclude(x => x.ProductBrand);

            var products = await _productRepo.ListAsync(spec);

            return Ok(_mapper
            .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProductAsync(int id)
        {
             var spec = new BaseSpecification<Product>(x => x.Id == id);
            spec.AddInclude(x => x.ProductType);
            spec.AddInclude(x => x.ProductBrand); 

            var product = await _productRepo.GetEntityWithSpec(spec);

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