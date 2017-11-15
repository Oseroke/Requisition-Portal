using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequisitionPortal.BL.Entities;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using System.Data;
using RequisitionPortal.BL.Utils;
using RequisitionPortal.BL.Abstracts;

namespace RequisitionPortal.BL.Infrastructure
{
    public partial class NHibernateRepository<T, TId> : IRepository<T, TId>
       where T : BaseEntity<TId>
       where TId : struct
    {
        NHibernate.ISession currentSession = NHibernateHelper.GetCurrentSession();
        public NHibernateRepository()
        {

        }

        public void Evict(T entity)
        {
            currentSession.Evict(entity);
        }
        public T GetById(object id)
        {

            return currentSession.Get<T>(id);
        }
        public IList<T> ExecuteStoredProcedureSelect(string StoreProcedureName, params object[] parameters)
        {



            List<string> str = new List<string>();
            for (int j = 0; j < parameters.Length; j++)
                str.Add("?");
            var St = String.Join(" , ", str);
            var query = currentSession.CreateSQLQuery("exec " + StoreProcedureName + " " + St);
            int i = 0;
            foreach (object obj in parameters)
            {
                query.SetParameter(i, obj);
                ++i;
            }


            var results = query.SetResultTransformer(new AliasToBeanResultTransformer(typeof(T)));

            return results.List<T>();

        }



        public DataTable ExecuteStoredProcedureSelectDataTable(string StoreProcedureName, params object[] parameters)
        {       

            List<string> str = new List<string>();
            for (int j = 0; j < parameters.Length; j++)
                str.Add("?");
            var St = String.Join(" , ", str);
            var query = currentSession.CreateSQLQuery("exec " + StoreProcedureName + " " + St);
            int i = 0;
            foreach (object obj in parameters)
            {
                query.SetParameter(i, obj);
                ++i;
            }



            var results = query.SetResultTransformer(new DataTableResultTransformer());
            return results.List()[0] as DataTable;


        }

        public void ExecuteStoredProcedureUpdate(string StoredName, params object[] parameters)
        {
            List<string> str = new List<string>();
            for (int j = 0; j < parameters.Length; j++)
                str.Add("?");
            var St = String.Join(" , ", str);

            var query = currentSession.CreateSQLQuery("exec " + StoredName + " " + St);
            int i = 0;
            foreach (object obj in parameters)
            {
                query.SetParameter(i, obj);
                ++i;
            }

            query.ExecuteUpdate();

        }

        public void StartTransaction()
        {
            currentSession.Transaction.Begin();
        }

        public void CommitTransaction()
        {
            currentSession.Transaction.Commit();
        }
        public void RollBackTransaction()
        {
            currentSession.Transaction.Rollback();
        }

        public void SaveOrUpdate(T entity)
        {
            //using (ITransaction trans = currentSession.BeginTransaction())
            //{
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("Invalid Object " + entity.GetType().Name);



                currentSession.SaveOrUpdate(entity);
                currentSession.Flush();
                currentSession.Refresh(entity);


                //trans.Commit();


            }
            catch (Exception ex)
            {

                // trans.Rollback();
                NHibernateHelper.CloseSession();
                throw new RequisitionException("Unable to save Entity of type : " + entity.GetType().Name + " REASON::: " + ex.Message);


            }
            // }
        }

        public void Flush()
        {
            currentSession.Clear();
            currentSession.Flush();
        }
        public void Delete(T entity)
        {
            using (ITransaction trans = currentSession.BeginTransaction())
            {
                try
                {
                    if (entity == null)
                        throw new ArgumentNullException("Invalid Object " + entity.GetType().Name);
                    currentSession.Delete(entity);

                    currentSession.Flush();

                    trans.Commit();
                }
                catch (Exception ex)
                {

                    trans.Rollback();
                    NHibernateHelper.CloseSession();
                    throw new RequisitionException("Unable to Delete Entity of type : " + entity.GetType().Name + " REASON::: " + ex.Message);

                }
            }
        }

        /// <summary>
        /// This returns all the Entities in a particular Type and transform to IQueryable
        /// </summary>
        public IQueryable<T> Table
        {
            get
            {

                return currentSession.Query<T>();
                
            }
            
        }
        

        public void Refresh(T entity)
        {
            currentSession.Refresh(entity);
        }
    }
}
