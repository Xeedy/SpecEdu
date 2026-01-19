using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace SpecEdu.Web.Tests.TestFixtures;

/// <summary>
/// Base class for page model tests providing common setup utilities.
/// </summary>
public abstract class PageModelTestBase
{
    /// <summary>
    /// Creates a PageContext with common test configuration.
    /// </summary>
    protected PageContext CreatePageContext()
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        return new PageContext(actionContext)
        {
            ViewData = viewData
        };
    }

    /// <summary>
    /// Creates a mock IUrlHelper that returns the expected URL pattern.
    /// </summary>
    protected IUrlHelper CreateMockUrlHelper(string contentResult = "~/")
    {
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns(contentResult);
        return urlHelper.Object;
    }

    /// <summary>
    /// Sets up a page model with common test configuration.
    /// </summary>
    protected void SetupPageModel(PageModel pageModel)
    {
        var httpContext = new DefaultHttpContext();
        var tempDataProvider = new Mock<ITempDataProvider>();

        pageModel.PageContext = CreatePageContext();
        pageModel.TempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
        pageModel.Url = CreateMockUrlHelper();
    }
}
