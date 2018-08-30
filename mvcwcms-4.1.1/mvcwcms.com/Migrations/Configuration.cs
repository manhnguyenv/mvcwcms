namespace MVCwCMS.Migrations
{
    using MVCwCMS.Data;
    using MVCwCMS.Models;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<StoreEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(StoreEntities context)
        {
            GetProductCategories().ForEach(c => context.ProductCategories.Add(c));
            GetProducts().ForEach(c => context.Products.Add(c));

            GetCategories().ForEach(c => context.Categories.Add(c));
            GetGadgets().ForEach(g => context.Gadgets.Add(g));

            context.Commit();
        }

        private static List<ProductCategory> GetProductCategories()
        {
            return new List<ProductCategory>
            {
                new ProductCategory {
                    Id = 1,
                    Name = "Mỹ phẩm"
                }
            };
        }

        private static List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product {
                    CategoryId = 1,
                    Name = "Sữa rửa mặt",
                    Price = 280000,
                    Code = "SRM"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Tẩy da chết sinh học",
                    Price = 280000,
                    Code = "TDCSH"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Kem nám",
                    Price = 650000,
                    Code = "KN"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Tắm trắng khô VV",
                    Price = 550000,
                    Code = "TTKVV"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Sữa tắm trắng da vàng 24K",
                    Price = 380000,
                    Code = "STTDV24K"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Tinh chất mụn (nhỏ)",
                    Price = 350000,
                    Code = "TCMN"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Tinh chất mụn (lớn)",
                    Price = 850000,
                    Code = "TCML"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Chống nắng sinh học",
                    Price = 450000,
                    Code = "CNSH"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Gel mờ thâm",
                    Price = 450000,
                    Code = "GMT"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Serum hạt vàng (nhỏ)",
                    Price = 250000,
                    Code = "SRHVN"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Mặt nạ cánh hoa hồng",
                    Price = 280000,
                    Code = "MNCHH"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Nước thần co lỗ chân lông",
                    Price = 350000,
                    Code = "NTCLCL"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Lăn mụn đầu đen",
                    Price = 250000,
                    Code = "LMDD"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Viên siêu dưỡng chất",
                    Price = 850000,
                    Code = "VSDC"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Phấn nước cushion",
                    Price = 380000,
                    Code = "PNCS"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Son tươi không chì",
                    Price = 280000,
                    Code = "STKC"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Kem body ngày",
                    Price = 650000,
                    Code = "KBDN"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Kem body đêm",
                    Price = 450000,
                    Code = "KBDD"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Mặt nạ detox thải độc",
                    Price = 550000,
                    Code = "MNDTTD"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Mặt nạ vàng 24K",
                    Price = 280000,
                    Code = "MNV24K"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Siêu chất 4 chức năng",
                    Price = 850000,
                    Code = "SC4CN"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Kem dưỡng đêm cao cấp",
                    Price = 750000,
                    Code = "KDDCC"
                },
                new Product {
                    CategoryId = 1,
                    Name = "Kem face lạnh",
                    Price = 450000,
                    Code = "KFL"
                }
            };
        }

        private static List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category {
                    Name = "Tablets"
                },
                new Category {
                    Name = "Laptops"
                },
                new Category {
                    Name = "Mobiles"
                }
            };
        }

        private static List<Gadget> GetGadgets()
        {
            return new List<Gadget>
            {
                new Gadget {
                    Name = "ProntoTec 7",
                    Description = "Android 4.4 KitKat Tablet PC, Cortex A8 1.2 GHz Dual Core Processor,512MB / 4GB,Dual Camera,G-Sensor (Black)",
                    CategoryID = 1,
                    Price = 46.99m,
                    Image = "prontotec.jpg"
                },
                new Gadget {
                    Name = "Samsung Galaxy",
                    Description = "Android 4.4 Kit Kat OS, 1.2 GHz quad-core processor",
                    CategoryID = 1,
                    Price = 120.95m,
                    Image= "samsung-galaxy.jpg"
                },
                new Gadget {
                    Name = "NeuTab® N7 Pro 7",
                    Description = "NeuTab N7 Pro tablet features the amazing powerful, Quad Core processor performs approximately Double multitasking running speed, and is more reliable than ever",
                    CategoryID = 1,
                    Price = 59.99m,
                    Image= "neutab.jpg"
                },
                new Gadget {
                    Name = "Dragon Touch® Y88X 7",
                    Description = "Dragon Touch Y88X tablet featuring the incredible powerful Allwinner Quad Core A33, up to four times faster CPU, ensures faster multitasking speed than ever. With the super-portable size, you get a robust power in a device that can be taken everywhere",
                    CategoryID = 1,
                    Price = 54.99m,
                    Image= "dragon-touch.jpg"
                },
                new Gadget {
                    Name = "Alldaymall A88X 7",
                    Description = "This Alldaymall tablet featuring the incredible powerful Allwinner Quad Core A33, up to four times faster CPU, ensures faster multitasking speed than ever. With the super-portable size, you get a robust power in a device that can be taken everywhere",
                    CategoryID = 1,
                    Price = 47.99m,
                    Image= "Alldaymall.jpg"
                },
                new Gadget {
                    Name = "ASUS MeMO",
                    Description = "Pad 7 ME170CX-A1-BK 7-Inch 16GB Tablet. Dual-Core Intel Atom Z2520 1.2GHz CPU",
                    CategoryID = 1,
                    Price = 94.99m,
                    Image= "asus-memo.jpg"
                },
                new Gadget {
                    Name = "ASUS 15.6-Inch",
                    Description = "Latest Generation Intel Dual Core Celeron 2.16 GHz Processor (turbo to 2.41 GHz)",
                    CategoryID = 2,
                    Price = 249.5m,
                    Image = "asus-latest.jpg"
                },
                new Gadget {
                    Name = "HP Pavilion 15-r030wm",
                    Description = "This Certified Refurbished product is manufacturer refurbished, shows limited or no wear, and includes all original accessories plus a 90-day warranty",
                    CategoryID = 2,
                    Price = 299.95m,
                    Image = "hp-pavilion.jpg"
                },
                new Gadget {
                    Name = "Dell Inspiron 15.6-Inch",
                    Description = "Intel Celeron N2830 Processor, 15.6-Inch Screen, Intel HD Graphics",
                    CategoryID = 2,
                    Price = 308.00m,
                    Image = "dell-inspiron.jpg"
                },
                new Gadget {
                    Name = "Acer Aspire E Notebook",
                    Description = "15.6 HD Active Matrix TFT Color LED (1366 x 768) 16:9 CineCrystal Display",
                    CategoryID = 2,
                    Price = 299.95m,
                    Image = "acer-aspire.jpg"
                },
                new Gadget {
                    Name = "HP Stream 13",
                    Description = "Intel Celeron N2840 Processor. 2 GB DDR3L SDRAM, 32 GB Solid-State Drive and 1TB OneDrive Cloud Storage for one year",
                    CategoryID = 2,
                    Price = 202.99m,
                    Image = "hp-stream.jpg"
                },
                new Gadget {
                    Name = "Nokia Lumia 521",
                    Description = "T-Mobile Cell Phone 4G - White. 5MP Camera - Snap creative photos with built-in digital lenses",
                    CategoryID = 3,
                    Price = 63.99m,
                    Image = "nokia-lumia.jpg"
                },
                new Gadget {
                    Name = "HTC Desire 816",
                    Description = "13 MP Rear Facing BSI Camera / 5 MP Front Facing",
                    CategoryID = 3,
                    Price = 177.99m,
                    Image = "htc-desire.jpg"
                },
                new Gadget {
                    Name = "Sanyo Innuendo",
                    Description = "Uniquely designed 3G-enabled messaging phone with side-flipping QWERTY keyboard and external glow-thru OLED dial pad that 'disappears' when not in use",
                    CategoryID = 3,
                    Price = 54.99m,
                    Image = "sanyo-innuendo.jpg"
                },
                new Gadget {
                    Name = "Ulefone N9000",
                    Description = "Unlocked world GSM phone. 3G-850/2100, 2G -850/900/1800/1900",
                    CategoryID = 3,
                    Price = 133.99m,
                    Image = "ulefone.jpg"
                }
            };
        }
    }
}