﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Moq;
using NUnit.Framework;
using LinkFinder.Logic.Crawlers;
using LinkFinder.Logic.Services;
using LinkFinder.Logic.Validators;
using LinkFinder.Logic.Models;

namespace LinkFinder.Logic.Tests
{
    class SitemapCrawlerTests
    {
        Mock<LinkValidator> mockLinkValidator;
        Mock<LinkConverter> mockLinkConverter;
        Mock<RequestService> mockRequestService;
        Mock<LinkParser> mockLinkParser;

        [SetUp]
        public void SetUp()
        {
            mockLinkValidator = new Mock<LinkValidator>();
            mockLinkConverter = new Mock<LinkConverter>();
            mockRequestService = new Mock<RequestService>();
            mockLinkParser = new Mock<LinkParser>();
            string errorMessage;

            mockLinkValidator.Setup(p => p.IsCorrectLink(It.IsAny<string>(),out errorMessage))
                             .Returns(true);
            mockLinkValidator.Setup(p => p.IsInCurrentSite(It.IsAny<string>(), It.IsAny<string>()))
                             .Returns(true);
            mockLinkValidator.Setup(p => p.IsFileLink(It.IsAny<string>()))
                             .Returns(false);
        }

        [Test]
        public void GetLinks_EmptyString__ReturnEmptyList()
        {
            //Arrange
            string errorMessage;
            mockLinkValidator.Setup(p => p.IsCorrectLink("",out errorMessage))
                             .Returns(false);
           
            mockLinkParser.Setup(p => p.Parse(It.IsAny<string>()))
                          .Returns(new List<string>());
           
            mockRequestService.Setup(p => p.DownloadPage(It.IsAny<Link>()))
                              .Returns("");
           
            mockLinkConverter.Setup(p => p.RelativeToAbsolute(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                             .Returns(new List<string> { });
            
            int timeResponse = 1;
            mockRequestService.Setup(p => p.SendRequest("", out timeResponse)).Returns(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest});

            SitemapCrawler sitemapCrawler = new SitemapCrawler(mockRequestService.Object, mockLinkConverter.Object, mockLinkParser.Object, mockLinkValidator.Object);

            //Act
            var result = sitemapCrawler.GetLinks("");

            //Assert
            Assert.AreEqual(result.Count(), 0);                 
        }


        [Test]
        public void GetLinks_SiteWithHaveCorrectSitemap_ReturnLinksList()
        {
            //Arrange
            mockRequestService.Setup(p => p.DownloadPage(new Link("https://example.com/sitemap.xml",0)))
                              .Returns("text<loc>https://example.com/</loc>text<loc>https://example.com/afraid</loc>text");

            mockLinkParser.Setup(p => p.Parse(It.IsAny<string>()))
                          .Returns(new List<string>() { "https://example.com/", "https://example.com/afraid" });
           
            mockLinkConverter.Setup(p => p.RelativeToAbsolute(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                             .Returns(new List<string>() { "https://example.com/", "https://example.com/afraid" });
          
            int timeResponse = 1;
            mockRequestService.Setup(p => p.SendRequest("", out timeResponse)).Returns(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });

            SitemapCrawler sitemapCrawler = new SitemapCrawler(mockRequestService.Object, mockLinkConverter.Object, mockLinkParser.Object, mockLinkValidator.Object);

            //Act
            var result = sitemapCrawler.GetLinks("https://example.com").ToList();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(result[0].Url, "https://example.com/");
                Assert.AreEqual(result[1].Url, "https://example.com/afraid");
            });
        }
        [Test]
        public void GetLinks_SiteWithNotHaveSitemap_ReturnEmptyList()
        {
            //Arrange
            mockLinkParser.Setup(p => p.Parse(It.IsAny<string>()))
                          .Returns(new List<string>());
            
            mockRequestService.Setup(p => p.DownloadPage(It.IsAny<Link>()))
                              .Returns("");
            
            mockLinkConverter.Setup(p => p.RelativeToAbsolute(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                             .Returns(new List<string> { });
            
            int timeResponse = 1;
            mockRequestService.Setup(p => p.SendRequest("", out timeResponse)).Returns(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest });

            SitemapCrawler sitemapCrawler = new SitemapCrawler(mockRequestService.Object, mockLinkConverter.Object, mockLinkParser.Object, mockLinkValidator.Object);

            //Act
            var result = sitemapCrawler.GetLinks("https://example.com/");

            //Assert
            Assert.AreEqual(result.Count(), 0);
        }
    }
}
