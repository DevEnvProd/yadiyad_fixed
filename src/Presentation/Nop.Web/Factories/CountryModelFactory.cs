﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Directory;
using YadiYad.Pro.Services.Common;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the country model factory
    /// </summary>
    public partial class CountryModelFactory : ICountryModelFactory
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly CityService _cityService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CountryModelFactory(ICountryService countryService,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            CityService cityService,
            IWorkContext workContext)
        {
            _countryService = countryService;
            _localizationService = localizationService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get states and provinces by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
        /// <returns>List of identifiers and names of states and provinces</returns>
        public virtual IList<StateProvinceModel> GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            if (string.IsNullOrEmpty(countryId))
                throw new ArgumentNullException(nameof(countryId));

            var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
            var states = _stateProvinceService
                .GetStateProvincesByCountryId(country?.Id ?? 0, _workContext.WorkingLanguage.Id)
                .ToList();
            var result = new List<StateProvinceModel>();
            foreach (var state in states)
                result.Add(new StateProvinceModel
                {
                    id = state.Id,
                    name = _localizationService.GetLocalized(state, x => x.Name)
                });

            if (country == null)
            {
                //country is not selected ("choose country" item)
                if (addSelectStateItem)
                {
                    result.Insert(0, new StateProvinceModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectState")
                    });
                }
                else
                {
                    result.Insert(0, new StateProvinceModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.Other")
                    });
                }
            }
            else
            {
                //some country is selected
                if (!result.Any())
                {
                    //country does not have states
                    result.Insert(0, new StateProvinceModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.Other")
                    });
                }
                else
                {
                    //country has some states
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new StateProvinceModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.SelectState")
                        });
                    }
                }
            }

            return result;
        }

        public virtual IList<CityModel> GetCitiesByStateId(string stateId, bool addSelectStateItem)
        {
            if (string.IsNullOrEmpty(stateId))
                throw new ArgumentNullException(nameof(stateId));

            var state = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateId));
            var cities = _cityService
                .GetCitiesByStateId(state?.Id ?? 0, _workContext.WorkingLanguage.Id)
                .ToList();
            var result = new List<CityModel>();
            foreach (var city in cities)
                result.Add(new CityModel
                {
                    id = city.Id,
                    name = city.Name
                });


            return result;
        }

        #endregion
    }
}
