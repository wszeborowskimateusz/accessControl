namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [StringLength(30)]
        public string login { get; set; }

        [StringLength(64)]
        public string password { get; set; }

        [StringLength(64)]
        public string salt { get; set; }

        public int levelOfPermissions { get; set; }
    }
}
