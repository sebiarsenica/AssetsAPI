using AssetsAPI.Classes;
using AssetsAPI.Data;
using AssetsAPI.DTOs;
using AssetsAPI.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignedAssetController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AssignedAssetController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<AssignedAsset>>> getAllAssignedAssets()
        {
            var all = await _context.AssignedAssets.Select(r => new AssignedAssetReturn
            {
                id = r.id,
                user = r.User.ConvertUserToUserReturn(),
                asset = r.Asset,
                assignedDate = r.assignedDate,
                expireDate = r.expireDate,
                status = r.status
            }).ToListAsync();

            return Ok(all);
        }

        [HttpPost]
        public async Task<ActionResult> addAssignedAsset(AssignedAssetDTO aadto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.id == aadto.UserId);
            var assetExists = await _context.Assets.AnyAsync(a => a.id == aadto.AssetId);

            if (!userExists || !assetExists) return BadRequest("UserId or AssetId does not exists");

            var userFind = await _context.Users.FindAsync(aadto.UserId);
            var assetFind = await _context.Assets.FindAsync(aadto.AssetId);

            var assignedAsset = new AssignedAsset();
            assignedAsset.UserId = aadto.UserId;
            assignedAsset.User = userFind;
            assignedAsset.AssetId = aadto.AssetId;
            assignedAsset.Asset = assetFind;

            string dateString = DateTime.Now.ToString("dd-MM-yyyy");
            assignedAsset.assignedDate = DateTime.ParseExact(dateString, "dd-MM-yyyy", CultureInfo.CurrentUICulture);
            assignedAsset.expireDate = aadto.expireDate;
            assignedAsset.status = "Pending";

            _context.AssignedAssets.Add(assignedAsset);
            await _context.SaveChangesAsync();  


            return Ok();
        }

        [HttpPost("request")]
        public async Task<ActionResult> requestAssignAssetAdd(AssignedAssetDTO aadto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.id == aadto.UserId);
            var assetExists = await _context.Assets.AnyAsync(a => a.id == aadto.AssetId);

            if (!userExists || !assetExists) return BadRequest("UserId or AssetId does not exists");

            var userFind = await _context.Users.FindAsync(aadto.UserId);
            var assetFind = await _context.Assets.FindAsync(aadto.AssetId);

            var assignedAsset = new AssignedAsset();
            assignedAsset.UserId = aadto.UserId;
            assignedAsset.User = userFind;
            assignedAsset.AssetId = aadto.AssetId;
            assignedAsset.Asset = assetFind;

            assignedAsset.status = "Pending";

            _context.AssignedAssets.Add(assignedAsset);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("editRequest")]
        public async Task<ActionResult<AssignedAssetReturn>> changeStatus(AssignedAssetReturn aar)
        {
            var AssignedAsset = await _context.AssignedAssets.FindAsync(aar.id);
            if (AssignedAsset == null) return BadRequest("AssignedAsset not found!");


            var assetFind = await _context.Assets.FindAsync(aar.asset.id);

            if (aar.status == "Approved" && assetFind.quantity - 1 >= 0)
                assetFind.quantity -= 1;
            else if (assetFind.quantity - 1 < 0)
                return BadRequest("The asset is no longer avaible");

            AssignedAsset.expireDate = aar.expireDate;
            AssignedAsset.status = aar.status;

            string dateString = DateTime.Now.ToString("dd-MM-yyyy");
            AssignedAsset.assignedDate = DateTime.ParseExact(dateString, "dd-MM-yyyy", CultureInfo.CurrentUICulture);

            await _context.SaveChangesAsync();

            return Ok(AssignedAsset);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteAssignAsset(int id)
        {
            bool noQuantity = false;
            var AssignedAsset = await _context.AssignedAssets.FindAsync(id);
            if (AssignedAsset == null) return BadRequest("AssignedAsset not found!");

            if (AssignedAsset.status == "Rejected")
                noQuantity = true;

            _context.AssignedAssets.Remove(AssignedAsset);

            
            var asset = await _context.Assets.FindAsync(AssignedAsset.AssetId);
            if (asset != null && noQuantity == false)
            {
                asset.quantity++; 
                _context.Assets.Update(asset);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> changeStatusRejected(int id)
        {
            var AssignedAsset = await _context.AssignedAssets.FindAsync(id);
            if (AssignedAsset == null) return BadRequest("AssignedAsset not found!");

            AssignedAsset.status = "Rejected";
            await _context.SaveChangesAsync(); 

            return Ok();
        }


    }
}
