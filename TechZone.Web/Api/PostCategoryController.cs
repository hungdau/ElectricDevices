using AutoMapper;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.Infrastructure.Core;
using TechZone.Web.Infrastructure.Extensions;
using TechZone.Web.Mappings;
using TechZone.Web.Models;

namespace TechZone.Web.Api
{
    [RoutePrefix("api/postcategory")]
    [Authorize]
    public class PostCategoryController : ApiControllerBase
    {
        private IPostCategoryService _postCategoryService;

        private IMappingService _mappingService;

        public PostCategoryController(IErrorService errorService, IPostCategoryService postCategoryService, IMappingService mappingService) :
            base(errorService)
        {
            this._postCategoryService = postCategoryService;
            this._mappingService = mappingService;
        }

        [Route("getall")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _postCategoryService.GetAll();
                var listPostCategoryVm = _mappingService.Mapper.Map<List<PostCategoryViewModel>>(model);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var content = new ObjectContent<List<PostCategoryViewModel>>(listPostCategoryVm, new JsonMediaTypeFormatter(), "application/json");
                response.Content = content;

                return response;
            });
        }

        [Route("add")]
        public HttpResponseMessage Post(HttpRequestMessage request, PostCategoryViewModel postCategoryVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (ModelState.IsValid)
                {
                    request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    PostCategory newPostCategory = new PostCategory();
                    newPostCategory.UpdatePostCategory(postCategoryVm);

                    var category = _postCategoryService.Add(newPostCategory);
                    _postCategoryService.Save();

                    response = request.CreateResponse(HttpStatusCode.Created, category);
                }
                return response;
            });
        }

        [Route("update")]
        public HttpResponseMessage Put(HttpRequestMessage request, PostCategoryViewModel postCategoryVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (ModelState.IsValid)
                {
                    request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var postCategoryDb = _postCategoryService.GetById(postCategoryVm.ID);
                    postCategoryDb.UpdatePostCategory(postCategoryVm);
                    _postCategoryService.Update(postCategoryDb);
                    _postCategoryService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK);
                }
                return response;
            });
        }

        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (ModelState.IsValid)
                {
                    request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    _postCategoryService.Delete(id);
                    _postCategoryService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK);
                }
                return response;
            });
        }
    }
}