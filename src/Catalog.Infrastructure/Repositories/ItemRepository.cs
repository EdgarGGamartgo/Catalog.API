using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Domain.Responses.Item;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories
{
    public class ItemRepository: IItemRepository
    {
        private readonly CatalogContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public ItemRepository(CatalogContext context)
        {
            _context = context??throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Item>> GetItemByArtistIdAsync(Guid id)
        {
            var items = await _context
                .Items
                .Where(item => item.ArtistId == id)
                .Include(x => x.Genre)
                .Include(x => x.Artist)
                .ToListAsync();
            return items;
        }

        public async Task<IEnumerable<Item>> GetItemByGenreIdAsync(Guid id)
        {
            var items = await _context.Items
                .Where(item => item.GenreId == id)
                .Include(x => x.Genre)
                .Include(x => x.Artist)
                .ToListAsync();

            return items;
        }

        public async Task<IEnumerable<Item>> GetAsync()
        {
            return await _context
                .Items
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Item> GetAsync(Guid id)
        {
            var item = await _context.Items
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Include(x => x.Genre)
                .Include(x=>x.Artist).FirstOrDefaultAsync();

                return item;
        }

        public Item Add(Item order)
        {
            return _context.Items
                .Add(order).Entity;
        }

        public Item Update(Item item)
        {
            _context.Entry(item).State = EntityState.Modified;
            return item;
        }
    }
}