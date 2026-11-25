using Microsoft.AspNetCore.Identity;

namespace portfolioApi.Models;

//gebruikersmodel voor de api
public class User : IdentityUser
{
    //erft standaard gebruikersgegevens van IdentityUser
    //(identity zoals id, email, username en passwordhash.)
}