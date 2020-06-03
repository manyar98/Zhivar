using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.ServiceLayer.BaseInfo;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using System.Net;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;
using System.Net.Http;
using Newtonsoft.Json;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.Utilities;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Helpers;
using System.Web.Http;
using Zhivar.ViewModel.Accunting;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Common;
using Zhivar.DataLayer.Validation;
using Zhivar.DataLayer.Validation.BaseInfo;

namespace Zhivar.Web.Controllers.BaseInfo
{
    [RoutePrefix("api/Saze")]
    public class SazeController : NewApiControllerBaseAsync<Saze, SazeVM>
    {
        public SazeRule Rule => this.BusinessRule as SazeRule;

        protected override IBusinessRuleBaseAsync<Saze> CreateBusinessRule()
        {
            return new SazeRule();
        }

        [Route("GetAll")]
        public  async Task<HttpResponseMessage> GetAll()
        {
            SazeRule sazeRule = new SazeRule();
            var list = await sazeRule.GetAllAsync();
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = list.ToList() });

        }

        [Route("GetAllByOrganId")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetAllByOrganId()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                SazeRule sazeRule = new SazeRule();
                var list = await sazeRule.GetAllByPersonIdAsync(Convert.ToInt32(organId));
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = list });
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        [Route("GetAllByGorohId")]
        public  async Task<HttpResponseMessage> GetAllByGorohId(int gorohId)
        {
            SazeRule sazeRule = new SazeRule();
            var list = await sazeRule.GetAllByGorohIdAsync(gorohId);
            var list2 = list.Select(x => new { ID = x.ID, Title = x.Title }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = list2.ToList() });

        }

        [HttpPost]
        [Route("GetNewObject")]
        public  async Task<HttpResponseMessage> GetNewObject()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            NewObjectSaze newObjectSaze = new NewObjectSaze()
            {
                item = new SazeVM()
                {
                    Address = "",
                    //Arz = 0,
                    Title = "",
                    //Tol = 0,
                    //NoeEjare = Enums.NoeEjare.Mahane,
                    //GoroheSazeID = -1,
                    //GoroheSazeName = "",
                    //ID = -1,
                    //Images = new List<SazeImageVM>(),
                    //NoeSazeId = 0,
                    Latitude = 0,
                    Longitude = 0,
                   // NoorDard = false,
                    //X = 0,
                    //////Y = 0,


                    Code = await CreateCodeSaze(organId),
                    ID = 0,

                        NodeGoroheSaze = new Node()
                        {
                            FamilyTree = "گروه رسانه",
                            Id = 2,
                            Name = "گروه رسانه"
                        },
                        NodeNoeSaze = new Node()
                        {
                            FamilyTree = "نوع رسانه",
                            Id = 4,
                            Name = "نوع رسانه"
                        },
                        NodeNoeEjare = new Node()
                        {
                            FamilyTree = "نوع اجاره",
                            Id = 5,
                            Name = "نوع اجاره"
                        },
                     
                        Images = new List<SazeImageVM>(),
                        
                    

                },

            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = newObjectSaze });
        }

        [HttpPost]
        [Route("GetSazeItem")]
        public  async Task<HttpResponseMessage> GetSazeItem([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            SazeRule sazeRule = new SazeRule();
            var saze = await sazeRule.GetSazeForEdit(id);

            NewObjectSaze newObjectSaze = new NewObjectSaze()
            {
                item = new SazeVM()
                {
                    Address = saze.Address,
                    Arz = saze.Arz,
                    Title = saze.Title,
                    Tol = saze.Tol,
                    OrganId = saze.OrganId,

                    //NoeEjare = Enums.NoeEjare.Mahane,
                    //GoroheSazeID = -1,
                    //GoroheSazeName = "",
                    //ID = -1,
                    //Images = new List<SazeImageVM>(),
                    //NoeSazeId = 0,
                    Latitude = saze.Latitude,
                    Longitude = saze.Longitude,
                    NoorDard = saze.NoorDard,
                    //X = 0,
                    //////Y = 0,


                    Code = saze.Code,
                    ID = saze.ID,

                    NodeGoroheSaze = new Node()
                    {
                        FamilyTree = saze.GoroheName,
                        Id = saze.GoroheSazeID,
                        Name = saze.GoroheName
                    },
                    NodeNoeSaze = new Node()
                    {
                        FamilyTree = saze.NoeSazeName,
                        Id = saze.NoeSazeId,
                        Name = saze.NoeSazeName
                    },
                    NodeNoeEjare = new Node()
                    {
                        FamilyTree = saze.NoeEjareName,
                        Id = saze.NoeEjareID,
                        Name = saze.NoeEjareName
                    },
                    ImagesCommon = saze.ImagesCommon



                },

            };

            if(newObjectSaze.item.ImagesCommon != null && newObjectSaze.item.ImagesCommon.Count> 0)
            {
                newObjectSaze.item.Images = new List<SazeImageVM>();

                foreach (var image in newObjectSaze.item.ImagesCommon)
                {

                    newObjectSaze.item.Images.Add(new SazeImageVM()
                    {
                        FileName = image.FileName,
                        FileSize = image.FileSize,
                        ID = image.ID,
                        IsDeleted = image.IsDeleted,
                        MimeType = image.MimeType,
                        Order = image.Order,
                        SazeId = image.SazeId,
                        TasvirBlobBase64 = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(image.Blob))
                    });
                }
            }
     

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = newObjectSaze });
        }

        [HttpPost]
        [Route("DeleteSazeItems")]
        public  async Task<HttpResponseMessage> DeleteSazeItems([FromBody] int[] ids)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                List<Saze> sazeLst = new List<Saze>();

                SazeRule sazeRule = new SazeRule();
                foreach (var id in ids)
                {
                    sazeLst.Add(await sazeRule.FindAsync(id));

                    sazeRule.Delete(id);
                    await sazeRule.SaveChangesAsync();
                }


                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = sazeLst });
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }
        private async Task<string> CreateCodeSaze(int organId)
        {

            var count = 0;
          //  GoroheSazeRule goroheSazeRule = new GoroheSazeRule();
           // var goroheSazelst = await goroheSazeRule.GetAllByOrganIdAsync(organId);

            SazeRule sazeRule = new SazeRule();
            var list = await sazeRule.GetAllByPersonIdAsync(Convert.ToInt32(organId));

            var saze = list.OrderByDescending(x => x.ID).FirstOrDefault();

            if(saze != null)
                count = Convert.ToInt32(saze.Code);
            //foreach (var goroheSaze in goroheSazelst ?? new List<GoroheSazeVM>())
            //{
            //    if(goroheSaze.Items != null && goroheSaze.Items.Any())
            //        count += goroheSaze.Items.Count();
            //}

            string code = "";
            count++;
            if (count < 10)
            {
                code = "00000" + count;
            }
            else if (count < 100)
            {
                code = "0000" + count;
            }
            else if (count < 1000)
            {
                code = "000" + count;
            }
            else if (count < 10000)
            {
                code = "00" + count;
            }
            else if (count < 100000)
            {
                code = "0" + count;
            }
            else
            {
                code = count.ToString();
            }

            return code;
        }


        [Route("Add")]
        [HttpPost]
        public async Task<HttpResponseMessage> Add(SazeVM sazeVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                //if (sazeVM.ID > 0)
                //{
                //    var deletedImages = this.BusinessRule.UnitOfWork.RepositoryAsync<SazeImage>().Queryable().Where(x => x.SazeId == sazeVM.ID).ToList();

                //    foreach (var deletedImage in deletedImages ?? new List<SazeImage>())
                //    {
                //        await this.BusinessRule.UnitOfWork.RepositoryAsync<SazeImage>().DeleteAsync(deletedImage.ID);
                //    }

                //}


                var saze = new Saze();

                foreach (var image in sazeVM.Images ?? new List<SazeImageVM>())
                {
                    if (image.TasvirBlobBase64 != null)
                    {
                        if (!string.IsNullOrWhiteSpace(image.TasvirBlobBase64) && !string.IsNullOrEmpty(image.TasvirBlobBase64))
                        {
                            image.TasvirBlobBase64 = image.TasvirBlobBase64.Replace("data:image/jpeg;base64,", "");
                            image.Blob = Convert.FromBase64String(image.TasvirBlobBase64);
                        }
                    }
                }

                sazeVM.GoroheSazeID = sazeVM.NodeGoroheSaze.Id;
                sazeVM.NoeEjareID = sazeVM.NodeNoeEjare.Id;
                sazeVM.NoeSazeId = sazeVM.NodeNoeSaze.Id;


                Mapper.Map(sazeVM, saze);

                if (saze.ID > 0)
                    saze.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                else
                    saze.ObjectState = OMF.Common.Enums.ObjectState.Added;

                SazeValidator validator = new SazeValidator();

                FluentValidation.Results.ValidationResult results = validator.Validate(saze);

                string failurs = "";

                if (!results.IsValid)
                {
                    foreach (var error in results.Errors)
                    {
                        failurs += "<br/>" + error.ErrorMessage;

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                }

                saze.NoeEjareID = sazeVM.NodeNoeEjare.Id;
                saze.NoeSazeId = sazeVM.NodeNoeSaze.Id;
                saze.GoroheSazeID = sazeVM.NodeGoroheSaze.Id;
                saze.OrganId = organId;

                foreach (var image in saze.Images ?? new List<SazeImage>())
                {
                    image.SazeId = saze.ID;
                    if (image.ID > 0)
                    {
                        image.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                    }
                    else
                    {
                        image.ObjectState = OMF.Common.Enums.ObjectState.Added;
                    }
                }

           


                Rule.InsertOrUpdateGraph(saze);
                await Rule.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = saze });

            }
            catch (Exception ex)
            {

                throw;
            }
       

        }

        //  [Route("Post")]
        [HttpPost]
        public  async Task<HttpResponseMessage> Post(SazeVM sazeVM)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = "" });


            var saze = new Saze();

            Mapper.Map(sazeVM, saze);
            
            Rule.Insert(saze);

            await Rule.SaveChangesAsync();

            // گرید آی دی جدید را به این صورت دریافت می‌کند

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = saze });



        }
        [HttpPut]
        // [Route("Update/{id:int}")]
        public  async Task<HttpResponseMessage> Update(int id, Saze saze)
        {
            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.NotFound, data = "" });



            if (!ModelState.IsValid || id != item.ID)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = "" });


            //  var goroheSaze = new GoroheSaze();
            //  Mapper.Map(goroheSazeVM, goroheSaze);

            Rule.Update(saze);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = saze });

        }
        //[HttpPut] 
        //public  HttpResponseMessage Update(int id, GoroheSaze product)
        //{
        //    var item = "12";
        //    if (item == null)
        //        return new HttpNotFoundResult();



        //    //Return HttpStatusCode.OK
        //    return new HttpStatusCodeResult(HttpStatusCode.OK);
        //}

        //[HttpDelete]
        //public  async Task<HttpResponseMessage> DeleteSaze(int id)
        //{

        //    var item = sazeRule.GetById(id);

        //    if (item == null)
        //        return new HttpNotFoundResult();

        //    sazeRule.Delete(id);

        //    await _unitOfWork.SaveAllChangesAsync();

        //    return Json(item);

        //}

        [HttpPost]
        //[Route("SaveImage}")]
        //public  HttpResponseMessage SaveImage(IEnumerable<HttpPostedFileBase> files)
        //{
        //    try
        //    {
        //        if (files != null)
        //        {
        //            foreach (var file in files)
        //            {
        //                string path = Path.Combine(Server.MapPath("~/UploadedFiles"), Path.GetFileName(file.FileName));
        //                file.SaveAs(path);
        //            }


        //        }
        //        //ViewBag.FileStatus = "File uploaded successfully.";
        //    }
        //    catch (Exception)
        //    {

        //        //ViewBag.FileStatus = "Error while file uploading.";
        //    }

        //    // Return an empty string to signify success

        //    return Content("");
        //}

        //        [HttpPost]
        //        [Route("RemoveImage}")]
        //        public  ContentResult RemoveImage(string[] fileNames)
        //        {
        //            if (fileNames != null)
        //            {
        //                foreach (var fullName in fileNames)
        //                {
        //                    // ...
        //                    // delete the files
        //                    // ...
        //                }
        //            }

        //            // Return an empty string to signify success
        //            return Content("");
        //        }


        private string getFilePath(string fileName, string path)
        {
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string newFullPath = Path.Combine(path, fileName); ;

            while (System.IO.File.Exists(newFullPath))
            {
                string tempFileName = $"{fileNameOnly}({count++})";
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return newFullPath;
        }


        //[HttpPost]
        //public  async Task<HttpResponseMessage> Save(IEnumerable<HttpPostedFileBase> files, int sazeId)
        //{
        //    // var lstUploadReults = new List<UploadFileResult>();
        //    var sazeImagelst = new List<SazeImage>();

        //    if (files != null)
        //    {
        //        foreach (var file in files)
        //        {
        //            if (file.ContentLength <= 0) continue;

        //            var guid = Guid.NewGuid();
        //            var fileExtension = Path.GetExtension(file.FileName);
        //            var fileName = $"{guid}{fileExtension}";
        //            var path = Server.MapPath(TmpPath) + fileName;
        //            file.SaveAs(path);

        //            var thumbnailFileName = $"{guid}-thumb{fileExtension}";

        //            var fileSize = file.ContentLength;

        //            GenerateProductThumbnailImage(file.InputStream, Server.MapPath(TmpPath) + thumbnailFileName);

        //            //lstUploadReults.Add(new UploadFileResult
        //            //{
        //            //    Url = Url.Content(TmpPath + fileName),
        //            //    Name = file.FileName,
        //            //    DeleteType = "Post",
        //            //    DeleteUrl = Url.Action(MVC.Upload.ActionNames.DeleteFile, MVC.Upload.Name, new { area = "" }),
        //            //    Size = fileSize,
        //            //    ThumbnailUrl = Url.Content(TmpPath + thumbnailFileName),
        //            //    Type = file.ContentType
        //            //});

        //            sazeImagelst.Add(new SazeImage()
        //            {
        //                Url = Url.Content(TmpPath + fileName),
        //                Name = file.FileName,
        //                DeleteUrl = Url.Action(MVC.Upload.ActionNames.DeleteFile, MVC.Upload.Name, new { area = "" }),
        //                Size = fileSize,
        //                ThumbnailUrl = Url.Content(TmpPath + thumbnailFileName),
        //                SazeId = sazeId,

        //            });




        //        }
        //    }

        //    var saze = await sazeRule.FindAsync(sazeId);

        //    if (saze.Images == null)
        //    {
        //        saze.Images = new List<SazeImage>();
        //        saze.Images = sazeImagelst;
        //    }
        //    else
        //    {
        //        foreach (var sazeImage in sazeImagelst)
        //        {
        //            saze.Images.Add(sazeImage);
        //        }

        //    }
        //    sazeRule.Update(saze);
        //    await _unitOfWork.SaveAllChangesAsync();
        //    // Return an empty string to signify success
        //    //return Json(new { Files = lstUploadReults });
        //    return Content("");
        //}

        //[HttpPost]
        //public  ContentResult Remove(string[] fileNames)
        //{
        //    if (fileNames != null)
        //    {
        //        foreach (var fullName in fileNames)
        //        {
        //            // ...
        //            // delete the files
        //            // ...
        //        }
        //    }

        //    // Return an empty string to signify success
        //    return Content("");
        //}
        private void GenerateProductThumbnailImage(Stream inputStream, string savePath)
        {
            var img = new WebImage(inputStream);
            img.Resize(181, 181, true, false).Crop(1, 1);

            img.Save(savePath);
        }

        //[HttpGet]
        //public  HttpResponseMessage map()
        //{
        //    ApplicationDBContext db = new ApplicationDBContext();
        //    var q = (from a in db.Locations
        //             select new { a.Name, a.Description, a.Latitude, a.Longitude }).OrderBy(a => a.Name);

        //    return Json(q, JsonRequestBehavior.AllowGet);
        //}

        public class UploadFileResult
        {
            public string Name { get; set; }
            public int Size { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public string DeleteUrl { get; set; }
            public string ThumbnailUrl { get; set; }
            public string DeleteType { get; set; }
        }
    }
}