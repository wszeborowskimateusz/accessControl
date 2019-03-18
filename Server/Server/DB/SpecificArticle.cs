namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SpecificArticle")]
    public partial class SpecificArticle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        public int sale_id { get; set; }

        public decimal? price { get; set; }

        [Required]
        [StringLength(20)]
        public string article_id { get; set; }

        public virtual Article Article { get; set; }

        public virtual Sale Sale { get; set; }
    }
}
