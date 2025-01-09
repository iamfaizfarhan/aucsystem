using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Aucsystem.Data;
using Aucsystem.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Aucsystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        // Redirecting to relevant role
        public async Task<IActionResult> RedirectToDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (await _userManager.IsInRoleAsync(user, "Seller"))
                return RedirectToAction(nameof(SellerDashboard));
            if (await _userManager.IsInRoleAsync(user, "Buyer"))
                return RedirectToAction(nameof(BuyerDashboard));

            return RedirectToAction("Index", "Home");
        }

        // Buyer Methods
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> BuyerDashboard()
        {
            var buyer = await _userManager.GetUserAsync(User);
            if (buyer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var bids = await _context.Bids
                .Include(b => b.Property)
                .Where(b => b.BuyerId == buyer.Id)
                .ToListAsync();

            return View(bids);
        }
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> SellerDashboard()
        {
            var seller = await _userManager.GetUserAsync(User);

            if (seller == null)
            {
                return RedirectToAction("Login", "Account");
            }

            #pragma warning disable CS8620
            var properties = await _context.Properties
                .Include(p => p.Bids)
                .ThenInclude(b => b.Buyer)
                .Where(p => p.SellerId == seller.Id)
                .ToListAsync();

            var auctions = await _context.Auctions
                .Include(a => a.Properties)
                .Where(a => a.Properties.Any(p => p.SellerId == seller.Id))
                .ToListAsync();

            #pragma warning disable CS8603
            var bids = properties
                .SelectMany(p => p.Bids)
                .Select(b => new BidViewModel
                {
                    BidId = b.BidId,
                    AuctionId = auctions.FirstOrDefault(a => a.Properties.Any(p => p.PropertyId == b.PropertyId))?.AuctionId ?? 0,
                    BuyerName = b.Buyer?.FullName,
                    BuyerEmail = b.Buyer?.Email,
                    PropertyId = b.PropertyId,
                    PropertyName = b.Property?.Name,
                    PropertyPrice = b.Property?.Price ?? 0,
                    Amount = b.Amount,
                    BidTime = b.BidTime
                })
                .ToList();

            var viewModel = new SellerDashboardViewModel
            {
                Properties = properties,
                Auctions = auctions,
                Bids = bids
            };

            return View(viewModel);
        }




        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ViewBids(int propertyId)
        {
            var seller = await _userManager.GetUserAsync(User);
            #pragma warning disable CS8602 
            var property = await _context.Properties
                .Include(p => p.Bids)
                .ThenInclude(b => b.Buyer)
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.SellerId == seller.Id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property.Bids);
        }

        // Property Methods (Add, Update, Edit, Delete)
        public IActionResult AddProperty() => View();

        [HttpPost]
        public async Task<IActionResult> AddProperty(Property property, List<IFormFile> Images)
        {
            var seller = await _userManager.GetUserAsync(User);

            #pragma warning disable CS8602 
            property.SellerId = seller.Id;

            if (Images != null && Images.Count > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);

                for (int i = 0; i < Images.Count && i < 3; i++)
                {
                    var image = Images[i];
                    if (image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        if (i == 0) property.ImageUrl1 = $"/images/{fileName}";
                        if (i == 1) property.ImageUrl2 = $"/images/{fileName}";
                        if (i == 2) property.ImageUrl3 = $"/images/{fileName}";
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Property added successfully!";
                return RedirectToAction(nameof(SellerDashboard));
            }

            return View(property);
        }

        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> EditProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            return View(property);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> EditProperty(Property property)
        {
            var existingProperty = await _context.Properties.FindAsync(property.PropertyId);
            if (existingProperty == null)
                return NotFound();

            existingProperty.Name = property.Name;
            existingProperty.Description = property.Description;
            existingProperty.Price = property.Price;
            existingProperty.Location = property.Location;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Property updated successfully!";
            return RedirectToAction(nameof(SellerDashboard));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Property deleted successfully!";
            return RedirectToAction(nameof(SellerDashboard));
        }

        public IActionResult PropertyPage(){
            return View();
        }

        [AllowAnonymous]
        [HttpGet("/Dashboard/PropertyPage/{id}")]
        public async Task<IActionResult> PropertyPage(int id)
        {
            #pragma warning disable CS8620
            var property = await _context.Properties
                .Include(p => p.Seller)
                .Include(p => p.Bids)
                .ThenInclude(b => b.Buyer)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return NotFound();

            var totalProperties = await _context.Properties.CountAsync(p => p.SellerId == property.SellerId);

            #pragma warning disable CS8604
            var viewModel = new PropertyPageViewModel
            {
                Property = property,
                Seller = property.Seller,
                TotalProperties = totalProperties,
                Bids = property.Bids.Select(b => new BidViewModel
                {
                    Amount = b.Amount,
                    BuyerName = b.Buyer?.FullName,
                    BuyerEmail = b.Buyer?.Email,
                    BidTime = b.BidTime
                }).ToList()
            };

            return View(viewModel);
        }

        // Bid Methods
        [HttpGet("/Dashboard/PlaceBid/{propertyId}")]
        public async Task<IActionResult> PlaceBid(int propertyId)
        {
            var property = await _context.Properties.FirstOrDefaultAsync(p => p.PropertyId == propertyId);

            if (property == null)
            {
                TempData["ErrorMessage"] = $"Property with ID {propertyId} not found.";
                return RedirectToAction("BuyerDashboard");
            }

            var viewModel = new PlaceBidViewModel
            {
                PropertyId = property.PropertyId,
                PropertyName = property.Name,
                CurrentPrice = property.Price
            };

            return View(viewModel);
        }

        [HttpPost("/Dashboard/PlaceBid")]
        public async Task<IActionResult> PlaceBid(PlaceBidViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "You need to log in to place a bid.";
                return RedirectToAction("Login", "Account");
            }

            var bid = new Bid
            {
                PropertyId = model.PropertyId,
                BuyerId = currentUser.Id,
                Amount = model.BidAmount,
                BidTime = DateTime.UtcNow
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bid placed successfully!";
            return RedirectToAction("PropertyPage", new { id = model.PropertyId });
        }

        // Contact Seller
        [AllowAnonymous]
        [HttpGet("/Dashboard/ContactSeller/{id}")]
        public async Task<IActionResult> ContactSeller(string id)
        {
            var seller = await _userManager.FindByIdAsync(id);
            if (seller == null)
            {
                return NotFound();
            }

            var totalProperties = await _context.Properties.CountAsync(p => p.SellerId == id);
            var listedProperties = await _context.Properties
                .Where(p => p.SellerId == id)
                .ToListAsync();

            var viewModel = new ContactSellerViewModel
            {
                FullName = seller.FullName,
                Email = seller.Email,
                PhoneNumber = seller.PhoneNumber,
                ProfilePicture = seller.ProfilePicture,
                TotalProperties = totalProperties,
                ListedProperties = listedProperties
            };

            return View(viewModel);
        }

        // Utility Methods
        public static string FormatPrice(decimal price)
        {
            if (price >= 1_000_000_000_000) return $"$AU {(price / 1_000_000_000_000):0.##}T";
            if (price >= 1_000_000_000) return $"$AU {(price / 1_000_000_000):0.##}B";
            if (price >= 1_000_000) return $"$AU {(price / 1_000_000):0.##}M";
            return $"$AU {price:n0}";
        }
        [HttpPost]
    
        [AllowAnonymous]
        public async Task<IActionResult> Auctions()
        {
            var auctions = await _context.Auctions
                .Include(a => a.Properties)
                .ThenInclude(p => p.Bids)
                .ToListAsync();

            return View(auctions ?? new List<Auction>());
        }
        
        [HttpGet]
        public IActionResult AddAuction(int ? propertyId){
             var sellerId = _userManager.GetUserId(User);
            var sellerProperties = _context.Properties
                .Where(p => p.SellerId == sellerId)
                .ToList();

            ViewBag.Properties = sellerProperties;

            if (propertyId.HasValue)
            {
                var selectedProperty = sellerProperties.FirstOrDefault(p => p.PropertyId == propertyId.Value);
                if (selectedProperty != null)
                {
                    ViewBag.SelectedPropertyId = selectedProperty.PropertyId;
                    ViewBag.SelectedPropertyName = selectedProperty.Name;
                    ViewBag.SelectedPropertyPrice = selectedProperty.Price;
                }
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddAuction(Auction auction, int propertyId)
        {
            var seller = await _userManager.GetUserAsync(User);
            if (seller == null)
            {
                TempData["ErrorMessage"] = "Seller not found.";
                return RedirectToAction("SellerDashboard");
            }

            if (auction.StartDate >= auction.EndDate)
            {
                TempData["ErrorMessage"] = "End date must be after start date.";
                return RedirectToAction("AddAuction", new { propertyId });
            }

            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.SellerId == seller.Id);

            if (property == null)
            {
                TempData["ErrorMessage"] = "Invalid property selected for auction.";
                return RedirectToAction("AddAuction");
            }

            // Associate the property with the auction
            auction.Properties = new List<Property> { property };

            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Auction created successfully!";
            return RedirectToAction(nameof(SellerDashboard));
        }


    [Authorize(Roles = "Buyer")]
    public async Task<IActionResult> ParticipateInAuction(int auctionId, int propertyId, decimal bidAmount)
    {
        var buyer = await _userManager.GetUserAsync(User);
        if (buyer == null)
        {
            TempData["ErrorMessage"] = "You need to log in to participate in auctions.";
            return RedirectToAction("Login", "Account");
        }

        var auction = await _context.Auctions
            .Include(a => a.Properties)
            .FirstOrDefaultAsync(a => a.AuctionId == auctionId);

        if (auction == null || !auction.Properties.Any(p => p.PropertyId == propertyId))
        {
            TempData["ErrorMessage"] = "Invalid auction or property.";
            return RedirectToAction("Auctions");
        }

        var bid = new Bid
        {
            PropertyId = propertyId,
            BuyerId = buyer.Id,
            Amount = bidAmount,
            BidTime = DateTime.UtcNow
        };

        _context.Bids.Add(bid);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Bid placed successfully!";
        return RedirectToAction("Auctions");
    }
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> EditAuction(int id)
    {
        var auction = await _context.Auctions.Include(a => a.Properties).FirstOrDefaultAsync(a => a.AuctionId == id);
        if (auction == null)
        {
            return NotFound();
        }

        var sellerId = _userManager.GetUserId(User);
        ViewBag.Properties = _context.Properties.Where(p => p.SellerId == sellerId).ToList();
        return View(auction);
    }

    [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> EditAuction(Auction auction, List<int> propertyIds)
    {
        var existingAuction = await _context.Auctions.Include(a => a.Properties).FirstOrDefaultAsync(a => a.AuctionId == auction.AuctionId);

        if (existingAuction == null)
        {
            return NotFound();
        }

        existingAuction.StartDate = auction.StartDate;
        existingAuction.EndDate = auction.EndDate;

        var properties = await _context.Properties
            .Where(p => propertyIds.Contains(p.PropertyId))
            .ToListAsync();

        existingAuction.Properties = properties;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Auction updated successfully!";
        return RedirectToAction(nameof(SellerDashboard));
    }

    [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> DeleteAuction(int auctionId)
    {
        var auction = await _context.Auctions
            .Include(a => a.Properties)
            .FirstOrDefaultAsync(a => a.AuctionId == auctionId);

        if (auction == null)
        {
            TempData["ErrorMessage"] = "Auction not found.";
            return RedirectToAction(nameof(SellerDashboard));
        }

        // Remove auction
        _context.Auctions.Remove(auction);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Auction deleted successfully!";
        return RedirectToAction(nameof(SellerDashboard));
    }
   [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> AcceptBid(int bidId)
    {
        var bid = await _context.Bids
            .Include(b => b.Property)
            .FirstOrDefaultAsync(b => b.BidId == bidId);

        if (bid == null)
        {
            TempData["ErrorMessage"] = "Bid not found.";
            return RedirectToAction(nameof(SellerDashboard));
        }

        var property = bid.Property;
        if (property != null)
        {
            property.IsSold = true; 
            _context.Properties.Update(property);
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Property has been sold!";
        return RedirectToAction(nameof(SellerDashboard));
    }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> RejectBid(int bidId)
        {
            var bid = await _context.Bids
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.BidId == bidId);

            if (bid == null)
            {
                TempData["ErrorMessage"] = "Bid not found.";
                return RedirectToAction("SellerDashboard");
            }

            _context.Bids.Remove(bid);
            TempData["SuccessMessage"] = $"Bid rejected for property: {bid.Property.Name}.";
            await _context.SaveChangesAsync();
            return RedirectToAction("SellerDashboard");
        }


    }

 
}
