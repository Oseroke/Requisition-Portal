﻿using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Abstracts
{
    public partial interface IRepository<T, TId>
    where T : BaseEntity<TId>
    where TId : struct
    {
        T GetById(object id);
        void SaveOrUpdate(T entity);
        void StartTransaction();
        void RollBackTransaction();
        void CommitTransaction();
        void Flush();
        void Evict(T entity);
        void Delete(T entity);
        void Refresh(T entity);
        IQueryable<T> Table { get; }

        IList<T> ExecuteStoredProcedureSelect(string StoreProcedureName, params object[] parameters);

        void ExecuteStoredProcedureUpdate(string StoredName, params object[] parameters);

        DataTable ExecuteStoredProcedureSelectDataTable(string StoreProcedureName, params object[] parameters);

        //static object ToType<T>(this object obj, T type);

        //static object ToNonAnonymousList<T>(this List<T> list, Type t);

        
    }

}
