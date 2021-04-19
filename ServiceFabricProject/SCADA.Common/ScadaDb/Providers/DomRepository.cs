using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Models;
using SCADA.Common.ScadaDb.Access;
using SCADA.Common.ScadaDb.Repositories;

namespace SCADA.Common.ScadaDb.Providers
{
    public class DomRepository : IDomRepository
    {
        private ScadaDbContext _context;
        private object _lockObject;

        public DomRepository(ScadaDbContext context) {
            _context = context;
            _lockObject = new object();
        }

        public void Add(List<DomDbModel> list)
        {
            lock (_lockObject)
            {
                using(var context = new ScadaDbContext())
                {
                    foreach (var model in list)
                    {
                        DomDbModel m = context.Dom.FirstOrDefault(d => d.Mrid == model.Mrid);

                        if (m == null)
                        {
                            context.Dom.Add(model);
                        }
                    }
                    context.SaveChanges();
                }
                
            }
            
        }

        public void AddOrUpdate(DomDbModel model)
        {
            lock (_lockObject)
            {
                using (var context = new ScadaDbContext())
                {
                    var all = context.Dom.ToList();
                    DomDbModel m = context.Dom.FirstOrDefault(d => !String.IsNullOrEmpty(d.Mrid) && d.Mrid == model.Mrid);

                    if (m == null)
                    {
                        context.Dom.Add(model);
                    }
                    else
                    {
                        m.ManipulationConut++;
                        m.TimeStamp = DateTime.Now.ToString();
                        context.Entry(m).State = System.Data.Entity.EntityState.Modified;
                    }

                    context.SaveChanges();
                }
                
            }
           
        }

        public void AddOrUpdateRange(List<DomDbModel> list)
        {
            lock (_lockObject)
            {
                using(var context = new ScadaDbContext())
                {
                    foreach (var model in list)
                    {
                        DomDbModel m = context.Dom.FirstOrDefault(d => !String.IsNullOrEmpty(d.Mrid) && d.Mrid == model.Mrid);

                        if (m == null)
                        {
                            context.Dom.Add(model);
                        }
                        else
                        {
                            m.ManipulationConut++;
                            m.TimeStamp = DateTime.Now.ToString();
                            context.Entry(m).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    context.SaveChanges();
                }
                
            }
            
        }

        public List<DomDbModel> GetAll()
        {
            List<DomDbModel> models = new List<DomDbModel>();
            lock (_lockObject)
            {
                using(var context = new ScadaDbContext())
                {
                    models = context.Dom.ToList();
                }
               
            }
            return models;
        }

        public void UpdateSingle(DomDbModel model)
        {
            lock (_lockObject)
            {
                using (var context = new ScadaDbContext())
                {
                    DomDbModel m = context.Dom.FirstOrDefault(d => !String.IsNullOrEmpty(d.Mrid) && d.Mrid == model.Mrid);

                    if (m != null)
                    {
                        m.ManipulationConut++;
                        m.TimeStamp = DateTime.Now.ToString();
                        context.Entry(m).State = System.Data.Entity.EntityState.Modified;
                    }


                    context.SaveChanges();
                }
                
            }
            
        }
    }
}
