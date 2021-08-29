using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Product 
    {
        [Key]
        public int Id {get; set;}

        [Required(ErrorMessage ="Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage ="Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        public string Title {get; set;}

        [MaxLength(1024, ErrorMessage ="Este campo deve conter entre 0 e 1024 caracteres")]
        public string Description {get; set;}

        [Required(ErrorMessage ="Este campo é obrigátorio")]
        [Range(1, int.MaxValue, ErrorMessage ="O preco deve ser maior q zero")]
        public decimal Price {get; set;}

        public int CategoryId {get; set;}

        public Category Category {get; set;}

    }
    
}
