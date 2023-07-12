using AssetsAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public StatisticsController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("counts")]
        public async Task<ActionResult<object>> getCounts()
        {
            var userCount = await _context.Users.CountAsync();
            var assetCount = await _context.Assets.CountAsync();
           
            DateTime startDate = DateTime.Today.AddDays(-30);
            var assignedAssetCount = await _context.AssignedAssets
                .Where(a => a.assignedDate >= startDate && a.assignedDate <= DateTime.Today)
                .CountAsync();

            var pendingAssignedAssetCount = await _context.AssignedAssets.CountAsync(x => x.status == "Pending");

            var counts = new
            {
                UserCount = userCount,
                AssetCount = assetCount,
                AssignedAssetCount = assignedAssetCount,
                PendingAssignedAssetCount = pendingAssignedAssetCount
            };

            return Ok(counts);
        }

        [HttpGet("category/{cat}")]
        public async Task<ActionResult<object>> getCategoryCounts(string cat)
        {
            var counts = new
            {
                Jan = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 1 && x.Asset.category == cat),
                Feb = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 2 && x.Asset.category == cat),
                Mar = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 3 && x.Asset.category == cat),
                Apr = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 4 && x.Asset.category == cat),
                May = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 5 && x.Asset.category == cat),
                June = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 6 && x.Asset.category == cat),
                July = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 7 && x.Asset.category == cat),
                Aug = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 8 && x.Asset.category == cat),
                Sept = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 9 && x.Asset.category == cat),
                Oct = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 10 && x.Asset.category == cat),
                Nov = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 11 && x.Asset.category == cat),
                Dec = await _context.AssignedAssets.CountAsync(x => x.assignedDate != default && x.assignedDate.Month == 12 && x.Asset.category == cat),
            };

            return Ok(counts);
        }



    }
}
