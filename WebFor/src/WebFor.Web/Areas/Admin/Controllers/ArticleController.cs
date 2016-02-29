﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using WebFor.Web.Areas.Admin.ViewModels;
using WebFor.Web.Areas.Admin.ViewModels.Article;
using WebFor.Web.Services;
using WebFor.Core.Domain;
using WebFor.Core.Repository;
using WebFor.Core.Services.ArticleServices;
using WebFor.Core.Services.Shared;


namespace WebFor.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private IUnitOfWork _uw;
        private ICkEditorFileUploder _ckEditorFileUploader;
        private IWebForMapper _webForMapper;
        private IArticleCreator _articleCreator;
        private IArticleEditor _articleEditor;

        public ArticleController(IUnitOfWork uw, ICkEditorFileUploder ckEditorFileUploader, IWebForMapper webForMapper, IArticleCreator articleCreator, IArticleEditor articleEditor)
        {
            _uw = uw;
            _ckEditorFileUploader = ckEditorFileUploader;
            _webForMapper = webForMapper;
            _articleCreator = articleCreator;
            _articleEditor = articleEditor;
        }

        public async Task<IActionResult> ManageArticle()
        {
            var articles = await _uw.ArticleRepository.GetAllAsync();

            var articlesViewModel = _webForMapper.ArticleCollectionToArticleViewModelCollection(articles);

            return View(articlesViewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var model = _webForMapper.ArticleViewModelToArticle(viewModel);

                int result = await _articleCreator.CreateNewArticleAsync(model, viewModel.ArticleTags);

                if (result > 0)
                {
                    TempData["ViewMessage"] = "مقاله با موفقیت ثبت شد.";

                    return RedirectToAction("ManageArticle", "Article");
                }

                TempData["ViewMessage"] = "مشکلی در ثبت مقاله پیش آمده، مقاله با موفقیت ثبت نشد.";

                return RedirectToAction("ManageArticle", "Article");
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                //return new HttpStatusCodeResult(StatusCodes.Status400BadRequest);
                return HttpBadRequest();
            }

            var article = await _uw.ArticleRepository.FindByIdAsync(id);

            if (article == null)
            {
                //return new HttpStatusCodeResult(StatusCodes.Status404NotFound);
                return HttpNotFound();
            }

            var articleViewModel = await _webForMapper.ArticleToArticleViewModelWithTagsAsync(article);

            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var article = _webForMapper.ArticleViewModelToArticle(viewModel);

                int result = await _articleEditor.EditArticleAsync(article, viewModel.ArticleTags);

                if (result > 0)
                {
                    TempData["ViewMessage"] = "مقاله با موفقیت ویرایش شد.";

                    return RedirectToAction("ManageArticle", "Article");
                }

                TempData["ViewMessage"] = "مشکلی در ویرایش مقاله پیش آمده، مقاله با موفقیت ثبت نشد.";

                return RedirectToAction("ManageArticle", "Article");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            return View();
        }


        public async Task<IActionResult> TagLookup()
        {
            var model = await _uw.ArticleTagRepository.GetAllTagNamesArrayAsync();

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> CkEditorFileUploder(IFormFile upload, string CKEditorFuncNum, string CKEditor,
           string langCode)
        {
            string vOutput = await _ckEditorFileUploader.UploadAsync(
                                   upload,
                                   new List<string>() { "Files", "ArticleUploads" },
                                   "/Files/ArticleUploads/",
                                   CKEditorFuncNum,
                                   CKEditor,
                                   langCode);

            return Content(vOutput, "text/html");
        }
    }
}
