﻿
using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Globalization;


/*
 * Want more WCF tips?
 * Visit http://webservices20.blogspot.com/
 */


namespace nCode.ServiceModel.BindingExtenions
{
    internal class ClearUsernameCollectionElement : StandardBindingCollectionElement<ClearUsernameBinding, ClearUsernameBindingElement>
    {
    }
}
