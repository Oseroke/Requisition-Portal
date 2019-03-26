using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Entities
{
    public class User: BaseEntity<int>
    {
        public virtual string Username { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string JobTitle { get; set; }
        public virtual string Department { get; set; }
        public virtual string Role { get; set; }

    }

    public class UserMap: ClassMapping<User>
    {
        public UserMap()
        {
            this.Lazy(true);
            this.Table("Users");
            this.Id<int>(x => x.Id, mp => { mp.Column("Id"); mp.Generator(Generators.Native); });
            this.Property<string>(x => x.Username, mp => { mp.Column("Username"); });
            this.Property<string>(x => x.FirstName, mp => { mp.Column("FirstName"); });
            this.Property<string>(x => x.LastName, mp => { mp.Column("LastName"); });
            this.Property<string>(x => x.Email, mp => { mp.Column("Email"); });
            this.Property<string>(x => x.DisplayName, mp => { mp.Column("DisplayName"); });
            this.Property<string>(x => x.JobTitle, mp => { mp.Column("JobTitle"); });
            this.Property<string>(x => x.Department, mp => { mp.Column("Department"); });
            this.Property<string>(x => x.Role, mp => { mp.Column("Role"); });
            this.Property<bool>(x => x.IsDeleted, mp => { mp.Column("IsDeleted"); });
        }

    }
}
