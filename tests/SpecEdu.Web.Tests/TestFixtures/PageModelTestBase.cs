using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace SpecEdu.Web.Tests.TestFixtures;

public abstract class PageModelTestBase
{
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

    protected IUrlHelper CreateMockUrlHelper(string contentResult = "~/")
    {
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns(contentResult);
        return urlHelper.Object;
    }

    protected void SetupPageModel(PageModel pageModel)
    {
        var httpContext = new DefaultHttpContext();
        var tempDataProvider = new Mock<ITempDataProvider>();

        pageModel.PageContext = CreatePageContext();
        pageModel.TempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
        pageModel.Url = CreateMockUrlHelper();
    }
}
