using System;
using System.Linq;
using System.Text;

namespace DbModel
{
    ///<summary>
    ///
    ///</summary>
    public partial class tb_areainfo
    {
           public tb_areainfo(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string areaCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string areaName {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string parentAreaCode {get;set;}

    }
}
