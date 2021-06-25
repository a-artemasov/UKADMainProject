﻿using LinkFinder.WebApi.RoutingParams;
using LinkFinder.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LinkFinder.WebApi.Controllers
{
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly TestsService _testsService;
        private readonly ResultsService _resultsService;

        public TestsController(TestsService testsService, ResultsService resultsService)
        {
            _testsService = testsService;
            _resultsService = resultsService;
        }

        /// <summary>
        /// Return all existing tests
        /// </summary>
        /// <returns></returns>
        [Route("api/tests")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var apiTests = await _testsService.GetAllTestsAsync();

            return Ok(apiTests);
        }

        /// <summary>
        /// Return detail information about test
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("api/test/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetTestResults(int id, [FromQuery] TestDetailParam param)
        {
            var results = await _testsService.GetTestAsync(id, param);

            return Ok(results);
        }

        /// <summary>
        /// Add new test for input site
        /// </summary>
        /// <param name="url">URL that will be crawled</param>
        /// <returns></returns>
        /// <response code="200">Website with input URL has been crawled</response>     
        /// <response code="400">When input URL not valid, return errorMessage </response>     
        [Route("api/test")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTestParam param)
        {
            var errorMessage = await _testsService.AddTestAsync(param.Url);

            if (String.IsNullOrEmpty(errorMessage) == false)
            {
                ModelState.AddModelError("errorMessage", errorMessage);

                return BadRequest(ModelState);
            }

            return Ok();
        }

        /// <summary>
        /// Return a count of results with test id
        /// </summary>
        /// <param name="id">Id of test which count results you want to get</param>
        /// <returns></returns>
        [Route("/api/test/{id}/count")]
        [HttpGet]
        public async Task<IActionResult> GetResultCount(int id)
        {
            var count = await _resultsService.GetResultCountAsync(id);

            return Ok(count);
        }
    }
}
