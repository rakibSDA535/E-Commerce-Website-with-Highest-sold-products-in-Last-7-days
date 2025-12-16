using CardCore.Models;
using CardCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CardCore.Controllers;
public class ProductController : Controller
{
    private readonly IProductRepository _productRepo;
    private readonly IGenreRepository _genreRepo;
    private readonly IFileService _fileService;

    public ProductController(IProductRepository productRepo, IGenreRepository genreRepo, IFileService fileService)
    {
        _productRepo = productRepo;
        _genreRepo = genreRepo;
        _fileService = fileService;
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> Index(string searchQuery, int page = 1, int pageSize = 5)
    {
        var query = _productRepo.GetAllProductsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(p => p.ProductName.Contains(searchQuery) || p.CompanyName.Contains(searchQuery));
        }
        int totalItems = await query.CountAsync();
        if (pageSize < 1) pageSize = 5;
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page > totalPages && totalPages > 0) page = totalPages;
        if (page < 1) page = 1;
        var products = await query
            .OrderBy(p => p.ProductName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        ViewBag.SearchQuery = searchQuery;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;

        return View(products);
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> AddProduct()
    {
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
        {
            Text = genre.GenreName,
            Value = genre.Id.ToString(),
        });
        ProductDTO productToAdd = new()
        {
            GenreList = genreSelectList,
            ReleaseDate = DateTime.Today,
            ProductInformations = new List<ProductInformationDTO>(),
            Stock = new StockDTO()//

        };
        return View(productToAdd);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductDTO productToAdd)
    {
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
        {
            Text = genre.GenreName,
            Value = genre.Id.ToString(),
        });
        productToAdd.GenreList = genreSelectList;
        productToAdd.ProductInformations = productToAdd.ProductInformations?
            .Where(d => !string.IsNullOrWhiteSpace(d.Description) || !string.IsNullOrWhiteSpace(d.MadeIn))
            .ToList() ?? new List<ProductInformationDTO>();

        if (!ModelState.IsValid)
            return View(productToAdd);

        try
        {
            if (productToAdd.ImageFile != null)
            {
                if (productToAdd.ImageFile.Length > 2 * 1024 * 1024)
                    throw new InvalidOperationException("Image file can not exceed 2 MB");

                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(productToAdd.ImageFile, allowedExtensions);
                productToAdd.Image = imageName;
            }

            Product product = new()
            {
                Id = productToAdd.Id,
                ProductName = productToAdd.ProductName,
                CompanyName = productToAdd.CompanyName,
                Image = productToAdd.Image,
                GenreId = productToAdd.GenreId,
                Price = productToAdd.Price,
                ReleaseDate = productToAdd.ReleaseDate,
                OnSale = productToAdd.OnSale,
            };
            foreach (var detail in productToAdd.ProductInformations)
            {
                product.ProductInformations.Add(new ProductInformation
                {
                    Description = detail.Description,
                    MadeIn = detail.MadeIn
                });
            }
            product.Stock = new Stock//
            {
                Quantity = productToAdd.Stock.Quantity,
            };

            await _productRepo.AddProduct(product);
            TempData["successMessage"] = "Product is added successfully";
            return RedirectToAction(nameof(AddProduct));
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(productToAdd);
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(productToAdd);
        }
        catch (Exception)
        {
            TempData["errorMessage"] = "Error on saving data";
            return View(productToAdd);
        }
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> UpdateProduct(int id)
    {
        var product = await _productRepo.GetProductById(id);
        if (product == null)
        {
            TempData["errorMessage"] = $"Product with the id: {id} does not found";
            return RedirectToAction(nameof(Index));
        }

        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
        {
            Text = genre.GenreName,
            Value = genre.Id.ToString(),
            Selected = genre.Id == product.GenreId
        });

        ProductDTO productToUpdate = new()
        {
            Id = product.Id,
            GenreList = genreSelectList,
            ProductName = product.ProductName,
            CompanyName = product.CompanyName,
            GenreId = product.GenreId,
            Price = product.Price,
            Image = product.Image,
            ReleaseDate = product.ReleaseDate,
            OnSale = product.OnSale,
            // Stock তথ্য DTO তে ম্যাপ করা হলো
            Stock = product.Stock != null ? new StockDTO
            {
                Id = product.Stock.Id,
                Quantity = product.Stock.Quantity
            } : new StockDTO(),
            ProductInformations = product.ProductInformations.Select(d => new ProductInformationDTO
            {
                Id = d.Id,
                Description = d.Description,
                MadeIn = d.MadeIn
            }).ToList()
        };

        return View(productToUpdate);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProduct(ProductDTO productToUpdate)
    {
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
        {
            Text = genre.GenreName,
            Value = genre.Id.ToString(),
            Selected = genre.Id == productToUpdate.GenreId
        });
        productToUpdate.GenreList = genreSelectList;
        productToUpdate.ProductInformations = productToUpdate.ProductInformations?
            .Where(d => !string.IsNullOrWhiteSpace(d.Description) || !string.IsNullOrWhiteSpace(d.MadeIn))
            .ToList() ?? new List<ProductInformationDTO>();

        if (!ModelState.IsValid)
            return View(productToUpdate);

        try
        {
            string oldImage = "";
            if (productToUpdate.ImageFile != null)
            {
                if (productToUpdate.ImageFile.Length > 2 * 1024 * 1024)
                    throw new InvalidOperationException("Image file can not exceed 2 MB");

                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(productToUpdate.ImageFile, allowedExtensions);
                oldImage = productToUpdate.Image;
                productToUpdate.Image = imageName;
            }

            var existingProduct = await _productRepo.GetProductById(productToUpdate.Id);
            if (existingProduct == null)
            {
                TempData["errorMessage"] = "Product not found";
                return RedirectToAction(nameof(Index));
            }
            existingProduct.ProductName = productToUpdate.ProductName;
            existingProduct.CompanyName = productToUpdate.CompanyName;
            existingProduct.GenreId = productToUpdate.GenreId;
            existingProduct.Price = productToUpdate.Price;
            existingProduct.ReleaseDate = productToUpdate.ReleaseDate;
            existingProduct.OnSale = productToUpdate.OnSale;
            existingProduct.Image = productToUpdate.Image ?? existingProduct.Image;

            // ৪. স্টকের পরিমাণ আপডেট করা - "existingProduct" ব্যবহার করা হয়েছে
            if (existingProduct.Stock != null)
            {
                existingProduct.Stock.Quantity = productToUpdate.Stock.Quantity;
            }
            else // যদি কোনো কারণে স্টক না থাকে, তবে নতুন স্টক তৈরি করা
            {
                // এটি সম্ভব যদি Product/Stock এর রিলেশনশিপ One-to-One হয় এবং Stock আগে তৈরি না হয়।
                existingProduct.Stock = new Stock
                {
                    Quantity = productToUpdate.Stock.Quantity,
                    // ProductId = existingProduct.Id // EF Core স্বয়ংক্রিয়ভাবে হ্যান্ডেল করবে
                };
            }

            var detailIds = productToUpdate.ProductInformations.Select(d => d.Id).ToList();
            var toRemove = existingProduct.ProductInformations.Where(d => !detailIds.Contains(d.Id)).ToList();

            foreach (var d in toRemove) existingProduct.ProductInformations.Remove(d);

            foreach (var detailVm in productToUpdate.ProductInformations)
            {
                if (detailVm.Id > 0)
                {
                    var existingDetail = existingProduct.ProductInformations.FirstOrDefault(d => d.Id == detailVm.Id);
                    if (existingDetail != null)
                    {
                        existingDetail.Description = detailVm.Description;
                        existingDetail.MadeIn = detailVm.MadeIn;
                    }
                }
                else
                {
                    existingProduct.ProductInformations.Add(new ProductInformation
                    {
                        Description = detailVm.Description,
                        MadeIn = detailVm.MadeIn
                    });
                }
            }

            await _productRepo.UpdateProduct(existingProduct);

            if (!string.IsNullOrWhiteSpace(oldImage))
                _fileService.DeleteFile(oldImage);

            TempData["successMessage"] = "Product is updated successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = $"Error on saving data: {ex.Message}";
            return View(productToUpdate);
        }
    }


    [Authorize(Roles = $"{nameof(Roles.User)}, {nameof(Roles.Admin)}")]
    public async Task<IActionResult> ProductDetails(int id)
    {
        var product = await _productRepo.GetProductById(id);

        if (product == null)
        {
            TempData["errorMessage"] = $"Product with ID {id} was not found.";
            return RedirectToAction(nameof(Index));
        }
        product.GenreName = product.Genre?.GenreName ?? "N/A";

        return View(product);
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _productRepo.GetProductById(id);
            if (product == null)
            {
                TempData["errorMessage"] = $"Product with the id: {id} does not found";
            }
            else
            {
                await _productRepo.DeleteProduct(product);

                if (!string.IsNullOrWhiteSpace(product.Image))
                    _fileService.DeleteFile(product.Image);
            }
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = $"Error on deleting data: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
    //For popup Details ckick on pic
    [HttpGet]
    // এই অ্যাকশনটি সাধারণত পাবলিক এক্সেসযোগ্য হতে পারে, কিন্তু যদি শুধু অথেন্টিকেটেড ব্যবহারকারীদের জন্য চান তবে Role যোগ করতে পারেন।
    // [Authorize(Roles = $"{nameof(Roles.User)}, {nameof(Roles.Admin)}")] 
    public async Task<IActionResult> GetProductDetailsPartial(int id)
    {
        // 1. Repository ব্যবহার করে পণ্যের বিস্তারিত ডেটা লোড করা
        var product = await _productRepo.GetProductById(id);

        if (product == null)
        {
            // পণ্য না পেলে HTTP 404 বা একটি সহজ ত্রুটি বার্তা ফেরত দেওয়া
            //return NotFound();
            // অথবা যদি আপনি HTML কোড ফেরত দিতে চান: 
             return Content("<div class='alert alert-danger'>Product details not found.</div>");
        }
        return PartialView("_ProductDetailsPartial", product);
    }

}
