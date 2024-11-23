    use("MongodbAspStory");

    db.createCollection("categories");
    db.categories.insertMany([
        {_id:1, catName:"Tu Tiên", catInformation:"A", catStatus:1},
        {_id:2, catName:"Trọng Sinh", catInformation:"A", catStatus:1},
        {_id:3, catName:"Huyền Huyễn", catInformation:"A", catStatus:1}
    ])

    db.createCollection("accounts");
    db.accounts.insertMany([
        {_id:1, fullName:"ADMIN", email:"admin@gmail.com", password:"123456", accPhone:"0987654321", accAddress:"Việt Nam", avatar:"beVoi.png", accStatus:"Đang hoạt động", accType:"admin"}
    ])

    db.createCollection("stories");
    db.stories.insertMany([
        {_id:1, title:"Thế Giới Hoàn Mỹ", author:"Thần Đông", interpreter:"Thiên", image:"the-gioi-hoan-my-truyen-chu.jpeg", stoInformation:"Bạn đang đọc truyện Thế Giới Hoàn Mỹ full (đã hoàn thành) của tác giả Thần Đông. Thạch Hạo, một thiếu niên với sức mạnh phi phàm chỉ dùng một tay liền có thể nhấc lên được cái đỉnh to. Năm xưa cậu được cha mẹ gửi đến cho tộc trưởng nơi Thạch Thôn. Từ đó tộc trưởng quyết tâm nuôi nấng cậu trưởng thành, rèn luyện cho cậu. Trong thôn có một cái cây là Liễu Thần, vào một ngày của mười sáu năm về trước chính là từ trên trời giáng xuống. Sau khi đại chiến với Lôi Đình Thịnh Nộ mới trở thành hình dáng hiện tại, bén rễ tại Thạch Thôn phù hộ thôn an bình. Thạch Hạo đặc biệt có cảm giác thân thuộc với Liễu Thần. Một hạt bụi cũng có thể lấp biển, một cọng cỏ xưng bá cả vũ trụ, chém mặt trời, mặt trăng và ngôi sao. Trong nháy mắt, làm chủ cả trời đất, giữa vô Vạn thiên nhai một mình ta độc bá. Vạn vật sinh sôi, mọc lên san sát như rừng cây, chư thánh tranh bá, thế gian loạn lạc, khắp đất trời nổi lên một hồi phong ba bão táp. Hỏi mặt đất bao la, hỏi trời cao rộng lớn, cuộc đời này là thế nào? Bên cạnh đó, còn có những truyện cùng thể loại khác cũng vô cùng hay và hấp dẫn như Già Thiên hay Thánh Khư.", stoStatus:"Hoàn thành", numberChap: 1100, catId:3},
        {_id:2, title:"Đấu Phá Thương Khung", author:"Thiên Tàm Thổ Đậu", interpreter:"Thiên", image:"dau-pha-thuong-khung-truyen-chu.jpg", stoInformation:"Bạn đang đọc truyện Đấu Phá Thương Khung full (đã hoàn thành) của tác giả Thiên Tàm Thổ Đậu. Tiêu Viêm, một thanh niên trẻ tràn đầy tự tin và được mọi người ngưỡng mộ với thiên phú tu luyện vượt trội. Nhưng một biến cố kinh hoàng đã đến, mẹ anh qua đời đột ngột, để lại cho anh một chiếc nhẫn màu đen bí ẩn. Từ lúc đó, số phận của Tiêu Viêm thay đổi hoàn toàn. Những ngày trước, anh là một thiếu niên tài năng, có khả năng tu luyện đỉnh cao, và anh tự hào về điều đó. Nhưng khi chiếc nhẫn đen kia xuất hiện trong cuộc sống của anh, mọi thứ đã thay đổi. Thiên phú tu luyện mà anh từng tự hào bỗng dưng biến mất, như một ánh sáng mặt trời bị che khuất bởi đám mây đen. Không còn tự tin và sức mạnh trước đây, Tiêu Viêm phải đối mặt với sự thật kỳ lạ và bí ẩn của chiếc nhẫn màu đen, và hành trình mới của anh bắt đầu từ đó. Từ đó cuộc đời của Tiêu Viêm có những biến hóa gì? Gặp được các đại ngộ gì? Thân phận thật sự của Huân Nhi (thanh mai trúc mã lúc nhỏ của Tiêu Viêm) ra sao? Bí mật của gia tộc hắn là gì? Cùng theo dõi bộ truyện này và còn có những bộ truyện cùng tác giả vô cùng hấp dẫn khác như Nguyên Tôn hay Đại Chúa Tể.", stoStatus:"Hoàn thành", numberChap: 1000, catId:3},
    ]);

    db.createCollection("chapters");
    db.createCollection("comments");
    db.createCollection("counters");
    db.counters.insertOne({
        _id: "categories", 
        seq: 3 // Set this to the last existing category ID + 1
    })
    db.counters.insertOne({
        _id: "accounts", 
        seq: 1 // Set this to the last existing category ID + 1
    })
    db.counters.insertOne({
        _id: "stories", 
        seq: 2 // Set this to the last existing category ID + 1
    })

    // KHÓA NGOẠI stories
    db.stories.aggregate([
        {
            $lookup: {
            from: "categories",
            localField: "catId",
            foreignField: "_id",
            as: "categories"
            }
        }
    ]).pretty()

    // KHÓA NGOẠI chapters
    db.chapters.aggregate([
        {
            $lookup: {
            from: "stories",
            localField: "stoId",
            foreignField: "_id",
            as: "stories"
            }
        }
    ]).pretty()

    // KHÓA NGOẠI comments
    db.comments.aggregate([
        {
            $lookup: {
            from: "stories",
            localField: "stoId",
            foreignField: "_id",
            as: "stories"
            }
        },
        {
            $lookup: {
            from: "accounts",
            localField: "accId",
            foreignField: "_id",
            as: "accounts"
            }
        }
    ]).pretty()