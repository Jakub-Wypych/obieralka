﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Services;
using Shared.Models.Dto;

namespace Shop.DB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : CrudController<Product, ProductDto, int>
    {
        public ProductsController(IProductService service, IMapper mapper)
            : base(service, mapper)
        {
        }
    }
}