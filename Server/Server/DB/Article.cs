namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Article")]
    public partial class Article
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Article()
        {
            SpecificArticles = new HashSet<SpecificArticle>();
        }

        [Key]
        [StringLength(20)]
        public string name { get; set; }

        [StringLength(50)]
        public string description { get; set; }

        public int producer_id { get; set; }

        public virtual Producer Producer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SpecificArticle> SpecificArticles { get; set; }
    }
}
