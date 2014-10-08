using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Utility class for Items
    /// </summary>
    public static class ItemUtilities
    {
        /// <summary>
        /// Gets the item title.
        /// </summary>
        /// <param name="itemId">The item unique identifier.</param>
        /// <param name="itemVariantId">The item variant unique identifier.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string GetItemTitle(Guid itemId, Guid? itemVariantId, string culture)
        {
            using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
            {
                string title = (from g in model.ItemLocalizations.Where(x => x.Culture == null)
                                join l in model.ItemLocalizations.Where(x => x.Culture == culture) on g.ItemID equals l.ItemID into localizedItems
                                from li in localizedItems.DefaultIfEmpty()
                                where g.ItemID == itemId
                                select (li ?? g).Title).Single();

                /* Append Variant Title */
                if (itemVariantId != null)
                {
                    List<string> variantNames = new List<string>();
                    while (itemVariantId != null)
                    {
                        var data = (from iv in model.ItemVariants
                                    join vg in model.VariantLocalizations.Where(x => x.Culture == null) on iv.VariantID equals vg.VariantID
                                    join vl in model.VariantLocalizations.Where(x => x.Culture == culture) on iv.VariantID equals vl.VariantID into vls
                                    where iv.ID == itemVariantId
                                    from vl in vls.DefaultIfEmpty()
                                    select new
                                    {
                                        ParentItemVariantID = iv.ParentID,
                                        (vl ?? vg).DisplayName
                                    }).Single();

                        variantNames.Add(data.DisplayName);
                        itemVariantId = data.ParentItemVariantID;
                    }

                    if (variantNames.Count > 0)
                    {
                        variantNames.Reverse();
                        title += " (" + String.Join(", ", variantNames) + ")";
                    }
                }

                return title;
            }
        }
    }
}
