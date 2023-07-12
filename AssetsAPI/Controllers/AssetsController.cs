using AssetsAPI.Classes;
using AssetsAPI.Data;
using AssetsAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing;

namespace AssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration; 

        public AssetsController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<Asset>>> getAllAssets()
        {
            var allAssets = await _context.Assets.Select(a => new Asset
            {
                id = a.id,
                name = a.name,
                sku= a.sku,
                category = a.category,
                quantity= a.quantity,
                addedBy= a.addedBy,
                image = a.image
            }).ToListAsync();

            return Ok(allAssets);
        }

        [HttpPost]
        public async Task<IActionResult> addAsset(AssetDTO assetDTO)
        {
            Image image; 
            using(MemoryStream ms = new MemoryStream(assetDTO.image))
            {
                image = Image.FromStream(ms);
            }

            int newWidth = 40;
            int newHeight = 40;
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);
            using(Graphics graphics = Graphics.FromImage(resizedImage))
            {
               graphics.DrawImage(image, 0, 0 , newWidth,newHeight);
            }

            byte[] resizedImageBytes; 
            using(MemoryStream ms = new MemoryStream())
            {
                resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                resizedImageBytes = ms.ToArray();   
            }
            
            var asset = new Asset();
            asset.name = assetDTO.name;

            var guid = Guid.NewGuid();

            asset.sku = guid.ToString("N").Substring(0, 5);
            asset.category = assetDTO.category;
            asset.quantity = assetDTO.quantity;
            asset.addedBy= assetDTO.addedBy;
            asset.image = resizedImageBytes;
            
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Asset>>> deleteAsset(int id)
        {
            var assetFound = _context.Assets.FirstOrDefault(a => a.id == id);
            if (assetFound == null) return BadRequest("Asset not found!");

            _context.Assets.Remove(assetFound);
            await _context.SaveChangesAsync();

            var allAssets = await _context.Assets.Select(a => new Asset
            {
                id = a.id,
                name = a.name,
                sku = a.sku,
                category = a.category,
                quantity = a.quantity,
                addedBy = a.addedBy,
                image = a.image
            }).ToListAsync();

            return Ok(allAssets);
        }

        [HttpPut]
        public async Task<ActionResult<string>> editAsset(Asset asset)
        {
            var assetFound = _context.Assets.FirstOrDefault(a => a.id == asset.id);
            if (assetFound == null) return BadRequest("Asset not found!");

            if(assetFound.name != asset.name) assetFound.name = asset.name;

            if(assetFound.quantity != asset.quantity) assetFound.quantity = asset.quantity;

            await _context.SaveChangesAsync();
            return Ok("Updated!");
        }

        [HttpPut("quantity/{id}")]
        public async Task<ActionResult> editQuantity(int id)
        {
            var assetFound = _context.Assets.FirstOrDefault(a => a.id == id);
            if (assetFound == null) return BadRequest("Asset not found!");
            if (assetFound.quantity == 0) return BadRequest("Quantity is 0");

            assetFound.quantity -= 1; 

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
