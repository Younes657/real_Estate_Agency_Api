using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class WebSiteInfo
    {
        [Key]
        public int Id { get; set; }
        public string? FacebookLink {  get; set; }
        public string? linkdinLink {  get; set; }
        public string? InstagramLink {  get; set; }
        public string? Email {  get; set; }
        public string? PhoneNumber {  get; set; }
        public string? TwitterLink { get; set; }
        public string? WhatUpNumber {  get; set; }

        [Required]
        public string Wilaya {  get; set; }
        [Required]
        public string Ville {  get; set; }
        [Required]
        public string Rue {  get; set; }
        public string? streetNumber { get; set; }
        public string? PostalCode {  get; set; }


    }
}
