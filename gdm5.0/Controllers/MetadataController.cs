using gdm5._0.Models;
using gdm5._0.Services;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Controllers
{
    [Route("api/metadata")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataService _metadataService;
        public MetadataController(IMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        // GET: api/Metadata/GetMetadata
        [HttpGet, Route("getmetadatatypes")]
        public IActionResult GetMetadataTypes()
        {

            return Ok(_metadataService.GetMetadataTypes());
        }
    }
}
