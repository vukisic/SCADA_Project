using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Access;
using SCADA.DB.Models;
using SCADA.DB.Repositories;

namespace SCADA.DB.Providers
{
    public class DomRepository : IDomRepository
    {
        private ScadaDbContext _context;

        public DomRepository(ScadaDbContext context) {
            _context = context;
        }

        public void AddOrUpdate(DomDbModel model)
        {
            DomDbModel m = _context.Dom.FirstOrDefault(d => d.Mrid == model.Mrid);

            if(m == null)
            {
                _context.Dom.Add(model);
            }
            else
            {
                m.ManipulationConut++;
                _context.Entry(m).State = System.Data.Entity.EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void AddOrUpdateRange(List<DomDbModel> list)
        {
            foreach(var model in list)
            {
                DomDbModel m = _context.Dom.FirstOrDefault(d => d.Mrid == model.Mrid);

                if (m == null)
                {
                    _context.Dom.Add(model);
                }
                else
                {
                    m.ManipulationConut++;
                    _context.Entry(m).State = System.Data.Entity.EntityState.Modified;
                }
            }
            _context.SaveChanges();
        }

        public List<DomDbModel> GetAll()
        {
            return _context.Dom.ToList();
        }

        public void UpdateSingle(DomDbModel model)
        {
            DomDbModel m = _context.Dom.FirstOrDefault(d => d.Mrid == model.Mrid);

            if (m != null)
            {
                m.ManipulationConut++;
                _context.Entry(m).State = System.Data.Entity.EntityState.Modified;
            }
                

            _context.SaveChanges();
        }
    }
}
