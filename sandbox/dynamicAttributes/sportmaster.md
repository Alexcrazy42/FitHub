вот json который спортмастер отдает метод
https://www.sportmaster.ru/web-api/v1/catalog/facets/

url: "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi"

проанализируй его и расскажи как у них реализовано отображение фасетов


{
    "pagination": {
        "page": 1,
        "pageSize": 36,
        "pagesCount": 1
    },
    "productCount": 11,
    "orderBy": "BY_POPULARITY",
    "facets": [
        {
            "id": "availability",
            "title": "РЎРїРѕСЃРѕР± РїРѕР»СѓС‡РµРЅРёСЏ",
            "displayType": "toggle",
            "displaySubType": "DELIVERY",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=delivery_wo_exp,exp_delivery&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "delivery",
                    "selectedByUser": false,
                    "isAvailable": true
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "availability",
            "title": "РЎРїРѕСЃРѕР± РїРѕР»СѓС‡РµРЅРёСЏ",
            "displayType": "one_of_list",
            "displaySubType": "TYPE_OF_PICKUP",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=pickup&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "pickup",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "Р—Р°РІС‚СЂР° РёР»Рё РїРѕР·Р¶Рµ",
                    "subQueryColorModelCount": 2
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=pickup_wo_supply&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "pickupWoSupply",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р§РµСЂРµР· 15 РјРёРЅСѓС‚",
                    "subQueryColorModelCount": 0
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "availability",
            "title": "РЎРїРѕСЃРѕР± РїРѕР»СѓС‡РµРЅРёСЏ",
            "displayType": "list",
            "displaySubType": "PICKUP",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=708&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "708",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "708",
                        "name": "РўР¦ В«РњР•Р“РђВ» Р”С‹Р±РµРЅРєРѕ",
                        "address": "Р›РµРЅРёРЅРіСЂР°РґСЃРєР°СЏ РѕР±Р»., Р’СЃРµРІРѕР»Р¶СЃРєРёР№ СЂ-РѕРЅ, Р—Р°РЅРµРІСЃРєРѕРµ РіРѕСЂРѕРґСЃРєРѕРµ РїРѕСЃРµР»РµРЅРёРµ, РљСѓРґСЂРѕРІРѕ, РњСѓСЂРјР°РЅСЃРєРѕРµ С€., 1",
                        "metroStations": [
                            {
                                "name": "РЈР»РёС†Р° Р”С‹Р±РµРЅРєРѕ",
                                "line": {
                                    "name": "РџСЂР°РІРѕР±РµСЂРµР¶РЅР°СЏ",
                                    "color": "#ea7125"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.8948589999999967403709888458251953125,
                            "lon": 30.51643899999999831607055966742336750030517578125
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=6596&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "6596",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "6596",
                        "name": "РўР Рљ \"Р‘Р°Р»РєР°РЅСЃРєРёР№\"",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР Рљ \"Р‘Р°Р»РєР°РЅСЃРєРёР№\", Р‘Р°Р»РєР°РЅСЃРєР°СЏ РїР»., Рґ. 5, Р»РёС‚. РђР”",
                        "metroStations": [
                            {
                                "name": "РљСѓРїС‡РёРЅРѕ",
                                "line": {
                                    "name": "РњРѕСЃРєРѕРІСЃРєРѕ-РџРµС‚СЂРѕРіСЂР°РґСЃРєР°СЏ",
                                    "color": "#0078c9"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "PRO",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.8287160000000000081854523159563541412353515625,
                            "lon": 30.37970899999999829788066563196480274200439453125
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=6215&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "6215",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "6215",
                        "name": "РўР¦ \"Р—Р°РЅРµРІСЃРєРёР№ РљР°СЃРєР°Рґ\"",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"Р—Р°РЅРµРІСЃРєРёР№ РљР°СЃРєР°Рґ\", Р—Р°РЅРµРІСЃРєРёР№ РїСЂ-С‚, Рґ.67, Рє.3, Р»РёС‚.Рђ",
                        "metroStations": [
                            {
                                "name": "Р›Р°РґРѕР¶СЃРєР°СЏ",
                                "line": {
                                    "name": "РџСЂР°РІРѕР±РµСЂРµР¶РЅР°СЏ",
                                    "color": "#ea7125"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.93276399999999881629264564253389835357666015625,
                            "lon": 30.438452999999999093461156007833778858184814453125
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=6983&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "6983",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "6983",
                        "name": "РўР¦ \"Р­РєРѕ РџР°СЂРє\"",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"Р­РєРѕ РџР°СЂРє\", Рі. РњСѓСЂРёРЅРѕ, РћС…С‚РёРЅСЃРєР°СЏ Р°Р»Р»РµСЏ, Рґ. 9",
                        "metroStations": [],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 60.04848100000000243881004280410706996917724609375,
                            "lon": 30.42383099999999984675014275126159191131591796875
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=715&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "715",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "715",
                        "name": "РўР¦ В«РњР•Р“РђВ» РџР°СЂРЅР°СЃ",
                        "address": "Р›РµРЅРёРЅРіСЂР°РґСЃРєР°СЏ РѕР±Р»., Р’СЃРµРІРѕР»РѕР¶СЃРєРёР№ СЂ-РЅ, Рґ. РџРѕСЂРѕС€РєРёРЅРѕ, РўР¦ \"РњР•Р“Рђ РџР°СЂРЅР°СЃ\", 117 РєРј РљРђР” , СЃС‚СЂ. 1",
                        "metroStations": [
                            {
                                "name": "РџР°СЂРЅР°СЃ",
                                "line": {
                                    "name": "РњРѕСЃРєРѕРІСЃРєРѕ-РџРµС‚СЂРѕРіСЂР°РґСЃРєР°СЏ",
                                    "color": "#0078c9"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 60.09053099999999858482624404132366180419921875,
                            "lon": 30.382684999999998609609974664635956287384033203125
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=719&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "719",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "719",
                        "name": "РўР¦ В«РСЋРЅСЊВ»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"РСЋРЅСЊ\", РїСЂ-С‚ РРЅРґСѓСЃС‚СЂРёР°Р»СЊРЅС‹Р№, Рґ.24, Р»РёС‚.Рђ",
                        "metroStations": [
                            {
                                "name": "Р›Р°РґРѕР¶СЃРєР°СЏ",
                                "line": {
                                    "name": "РџСЂР°РІРѕР±РµСЂРµР¶РЅР°СЏ",
                                    "color": "#ea7125"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.94648500000000268528310698457062244415283203125,
                            "lon": 30.47440100000000029467628337442874908447265625
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=5520&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "5520",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "5520",
                        "name": "РўР¦ В«РЁРєРёРїРµСЂСЃРєРёР№ РњРѕР»Р»В»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"РЁРєРёРїРµСЂСЃРєРёР№ РњРѕР»Р»\", РњР°Р»С‹Р№ РїСЂ-С‚ Р’.Рѕ., Рґ. 88, Р»РёС‚. Рђ",
                        "metroStations": [
                            {
                                "name": "РџСЂРёРјРѕСЂСЃРєР°СЏ",
                                "line": {
                                    "name": "РќРµРІСЃРєРѕ-Р’Р°СЃРёР»РµРѕСЃС‚СЂРѕРІСЃРєР°СЏ",
                                    "color": "#009a49"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.93821799999999910824044491164386272430419921875,
                            "lon": 30.2309249999999991587174008600413799285888671875
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=400&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "400",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "400",
                        "name": "РњРѕСЃРєРѕРІСЃРєРёР№ РїСЂ-С‚, 10",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РїСЂ-С‚ РњРѕСЃРєРѕРІСЃРєРёР№, Рґ.10/12, Р»РёС‚.Р’",
                        "metroStations": [
                            {
                                "name": "РЎР°РґРѕРІР°СЏ",
                                "line": {
                                    "name": "Р¤СЂСѓРЅР·РµРЅСЃРєРѕ-РџСЂРёРјРѕСЂСЃРєР°СЏ",
                                    "color": "#702785"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.9237660000000005311449058353900909423828125,
                            "lon": 30.31794599999999917372406343929469585418701171875
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=210&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "210",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "210",
                        "name": "РўР¦ В«Р“Р°Р»РµСЂРµСЏВ»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"Р“Р°Р»РµСЂРµСЏ\", Р›РёРіРѕРІСЃРєРёР№ РїСЂ-С‚, Рґ. 30Р°",
                        "metroStations": [
                            {
                                "name": "РџР»РѕС‰Р°РґСЊ Р’РѕСЃСЃС‚Р°РЅРёСЏ",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "23:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "23:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "23:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "23:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "23:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "23:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "23:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.92761399999999838428266230039298534393310546875,
                            "lon": 30.36068900000000070349415182135999202728271484375
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=725&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "725",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "725",
                        "name": "РўР¦ В«РџРёС‚РµСЂ Р Р°РґСѓРіР°В»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"РџРёС‚РµСЂ Р Р°РґСѓРіР°\", РїСЂ-С‚ РљРѕСЃРјРѕРЅР°РІС‚РѕРІ, Рґ.14, Р»РёС‚.Рђ",
                        "metroStations": [
                            {
                                "name": "РџР°СЂРє РџРѕР±РµРґС‹",
                                "line": {
                                    "name": "РњРѕСЃРєРѕРІСЃРєРѕ-РџРµС‚СЂРѕРіСЂР°РґСЃРєР°СЏ",
                                    "color": "#0078c9"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.867874999999997953636921010911464691162109375,
                            "lon": 30.3510160000000013269527698867022991180419921875
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=406&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "406",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "406",
                        "name": "Р“СЂР°Р¶РґР°РЅСЃРєРёР№ РїСЂ-С‚, 31",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РїСЂ-С‚ Р“СЂР°Р¶РґР°РЅСЃРєРёР№, Рґ.31, Р»РёС‚.Рђ",
                        "metroStations": [
                            {
                                "name": "РђРєР°РґРµРјРёС‡РµСЃРєР°СЏ",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 60.0079530000000005429683369584381580352783203125,
                            "lon": 30.39348499999999830833985470235347747802734375
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=410&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "410",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "410",
                        "name": "РўР Рљ В«РњРµСЂРєСѓСЂРёР№В»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР Рљ \"РњРµСЂРєСѓСЂРёР№\", СѓР». РЎР°РІСѓС€РєРёРЅР°, Рґ.141, Р»РёС‚.Рђ",
                        "metroStations": [
                            {
                                "name": "РЎС‚Р°СЂР°СЏ Р”РµСЂРµРІРЅСЏ",
                                "line": {
                                    "name": "Р¤СЂСѓРЅР·РµРЅСЃРєРѕ-РџСЂРёРјРѕСЂСЃРєР°СЏ",
                                    "color": "#702785"
                                }
                            },
                            {
                                "name": "Р‘РµРіРѕРІР°СЏ",
                                "line": {
                                    "name": "РќРµРІСЃРєРѕ-Р’Р°СЃРёР»РµРѕСЃС‚СЂРѕРІСЃРєР°СЏ",
                                    "color": "#009a49"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.99136299999999977217157720588147640228271484375,
                            "lon": 30.206534000000001327634890913031995296478271484375
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=5595&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "5595",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "5595",
                        "name": "РўР¦ В«Р–РµРјС‡СѓР¶РЅР°СЏ РџР»Р°Р·Р°В»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР Рљ \"Р–РµРјС‡СѓР¶РЅР°СЏ РїР»Р°Р·Р°\", РџРµС‚РµСЂРіРѕС„СЃРєРѕРµ С€., Рґ.51, Р»РёС‚.Рђ",
                        "metroStations": [
                            {
                                "name": "РђРІС‚РѕРІРѕ",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            },
                            {
                                "name": "Р›РµРЅРёРЅСЃРєРёР№ РїСЂРѕСЃРїРµРєС‚",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.84894299999999844885678612627089023590087890625,
                            "lon": 30.14573800000000147747414303012192249298095703125
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=411&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "411",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "411",
                        "name": "РРІР°РЅРѕРІСЃРєР°СЏ СѓР»., 6",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РРІР°РЅРѕРІСЃРєР°СЏ СѓР»., Рґ. 6, Р»РёС‚. Рђ",
                        "metroStations": [
                            {
                                "name": "Р›РѕРјРѕРЅРѕСЃРѕРІСЃРєР°СЏ",
                                "line": {
                                    "name": "РќРµРІСЃРєРѕ-Р’Р°СЃРёР»РµРѕСЃС‚СЂРѕРІСЃРєР°СЏ",
                                    "color": "#009a49"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.87565699999999679903339711017906665802001953125,
                            "lon": 30.447444999999998316297933342866599559783935546875
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=8541&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "8541",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "8541",
                        "name": "РўР¦ \"РћС…С‚Р° РјРѕР»Р»\"",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"РћС…С‚Р° РјРѕР»Р»\", Р‘СЂР°РЅС‚РѕРІСЃРєР°СЏ РґРѕСЂРѕРіР°, Рґ.3",
                        "metroStations": [],
                        "type": "SHOP",
                        "typeName": "PRO",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.94024399999999985766407917253673076629638671875,
                            "lon": 30.416527999999999565261532552540302276611328125
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=5150&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "5150",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "5150",
                        "name": "Р›РµРЅРёРЅСЃРєРёР№ РїСЂ-С‚, 127",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РїСЂ-С‚ Р›РµРЅРёРЅСЃРєРёР№, Рґ.127, Р»РёС‚.Р‘",
                        "metroStations": [
                            {
                                "name": "Р›РµРЅРёРЅСЃРєРёР№ РїСЂРѕСЃРїРµРєС‚",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "Р“РёРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.851394999999996571204974316060543060302734375,
                            "lon": 30.2655409999999989167918101884424686431884765625
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=6495&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "6495",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "6495",
                        "name": "РўР¦ \"РЎРёС‚Рё РњРѕР»Р»\"",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"РЎРёС‚Рё РњРѕР»Р»\", РїСЂ-С‚ РСЃРїС‹С‚Р°С‚РµР»РµР№, Рґ.5, Рє.2, Р»РёС‚.Рђ",
                        "metroStations": [
                            {
                                "name": "РџРёРѕРЅРµСЂСЃРєР°СЏ",
                                "line": {
                                    "name": "РњРѕСЃРєРѕРІСЃРєРѕ-РџРµС‚СЂРѕРіСЂР°РґСЃРєР°СЏ",
                                    "color": "#0078c9"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "PRO",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 60.00379000000000218051354750059545040130615234375,
                            "lon": 30.301393999999998385419530677609145641326904296875
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=735&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "735",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "735",
                        "name": "РџСѓР»РєРѕРІСЃРєРѕРµ С€РѕСЃСЃРµ Рґ. 19Р‘",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РџСѓР»РєРѕРІСЃРєРѕРµ С€РѕСЃСЃРµ Рґ. 19Р‘",
                        "metroStations": [
                            {
                                "name": "РњРѕСЃРєРѕРІСЃРєР°СЏ",
                                "line": {
                                    "name": "РњРѕСЃРєРѕРІСЃРєРѕ-РџРµС‚СЂРѕРіСЂР°РґСЃРєР°СЏ",
                                    "color": "#0078c9"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "PRO",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.825299000000001115040504373610019683837890625,
                            "lon": 30.3193950000000000954969436861574649810791015625
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=5985&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "5985",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "5985",
                        "name": "РўР¦ В«Р•РІСЂРѕРїРѕР»РёСЃВ»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР Р¦ \"Р•РІСЂРѕРїРѕР»РёСЃ\", РџРѕР»СЋСЃС‚СЂРѕРІСЃРєРёР№ РїСЂРѕСЃРїРµРєС‚, 84 Р»РёС‚.A",
                        "metroStations": [
                            {
                                "name": "Р›РµСЃРЅР°СЏ",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.98734499999999769670466776005923748016357421875,
                            "lon": 30.354265000000001606395017006434500217437744140625
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=5667&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "5667",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "5667",
                        "name": "РўР¦ В«Р›РѕРЅРґРѕРЅ РњРѕР»Р»В»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"Р›РѕРЅРґРѕРЅ РњРѕР»Р»\", СѓР». РљРѕР»Р»РѕРЅС‚Р°Р№, Рґ.3, Р»РёС‚.Р’",
                        "metroStations": [
                            {
                                "name": "РџСЂРѕСЃРїРµРєС‚ Р‘РѕР»СЊС€РµРІРёРєРѕРІ",
                                "line": {
                                    "name": "РџСЂР°РІРѕР±РµСЂРµР¶РЅР°СЏ",
                                    "color": "#ea7125"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.91096300000000240970621234737336635589599609375,
                            "lon": 30.446023000000000280351741821505129337310791015625
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=6954&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "6954",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "6954",
                        "name": "РўР¦ \"Р“СЂР°РЅРґ РљР°РЅСЊРѕРЅ\"",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"Р“СЂР°РЅРґ РљР°РЅСЊРѕРЅ\", РїСЂ-С‚ Р­РЅРіРµР»СЊСЃР°, Рґ. 154Рђ",
                        "metroStations": [
                            {
                                "name": "РџСЂРѕСЃРїРµРєС‚ РџСЂРѕСЃРІРµС‰РµРЅРёСЏ",
                                "line": {
                                    "name": "РњРѕСЃРєРѕРІСЃРєРѕ-РџРµС‚СЂРѕРіСЂР°РґСЃРєР°СЏ",
                                    "color": "#0078c9"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "PRO",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 60.059786000000002559318090789020061492919921875,
                            "lon": 30.334810000000000940190147957764565944671630859375
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=5105&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "5105",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "5105",
                        "name": "РўР¦ В«РћРєР°В»",
                        "address": "Рі. РљРѕР»РїРёРЅРѕ, РўР¦ \"РћРєР°\", СѓР». РћРєС‚СЏР±СЂСЊСЃРєР°СЏ, Рґ. 8, Р»РёС‚. Рђ",
                        "metroStations": [],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.73946500000000270347300102002918720245361328125,
                            "lon": 30.62155500000000074578565545380115509033203125
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=6197&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "6197",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "6197",
                        "name": "РўР¦ \"РџСЏС‚СЊ РћР·РµСЂ\"",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўР¦ \"РџСЏС‚СЊ РћР·РµСЂ\", СѓР».Р”РѕР»РіРѕРѕР·РµСЂРЅР°СЏ, Рґ.14, Рє.2, Р»РёС‚.Рђ",
                        "metroStations": [
                            {
                                "name": "РљРѕРјРµРЅРґР°РЅС‚СЃРєРёР№ РїСЂРѕСЃРїРµРєС‚",
                                "line": {
                                    "name": "Р¤СЂСѓРЅР·РµРЅСЃРєРѕ-РџСЂРёРјРѕСЂСЃРєР°СЏ",
                                    "color": "#702785"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 60.016964000000001533408067189157009124755859375,
                            "lon": 30.2465389999999985093381837941706180572509765625
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=6264&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "6264",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "6264",
                        "name": "РўР¦ \"CUBUS\"",
                        "address": "Рі. Р“Р°С‚С‡РёРЅР°, РўР¦ \"CUBUS\", РџСѓС€РєРёРЅСЃРєРѕРµ С€., Рґ.15",
                        "metroStations": [],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "21:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 59.58206899999999706096787122078239917755126953125,
                            "lon": 30.148593000000001751459421939216554164886474609375
                        }
                    }
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-availability=5051&f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "5051",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "shop": {
                        "number": "5051",
                        "name": "РўРљ В«Р РѕРґРµРѕ Р”СЂР°Р№РІВ»",
                        "address": "Рі. РЎР°РЅРєС‚-РџРµС‚РµСЂР±СѓСЂРі, РўРљ \"Р РѕРґРµРѕ Р”СЂР°Р№РІ\", РїСЂ-С‚ РљСѓР»СЊС‚СѓСЂС‹, Рґ. 1 Рђ",
                        "metroStations": [
                            {
                                "name": "Р“СЂР°Р¶РґР°РЅСЃРєРёР№ РїСЂРѕСЃРїРµРєС‚",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            },
                            {
                                "name": "РђРєР°РґРµРјРёС‡РµСЃРєР°СЏ",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            },
                            {
                                "name": "РџРѕР»РёС‚РµС…РЅРёС‡РµСЃРєР°СЏ",
                                "line": {
                                    "name": "РљРёСЂРѕРІСЃРєРѕ-Р’С‹Р±РѕСЂРіСЃРєР°СЏ",
                                    "color": "#d6083b"
                                }
                            },
                            {
                                "name": "РћР·РµСЂРєРё",
                                "line": {
                                    "name": "РњРѕСЃРєРѕРІСЃРєРѕ-РџРµС‚СЂРѕРіСЂР°РґСЃРєР°СЏ",
                                    "color": "#0078c9"
                                }
                            }
                        ],
                        "type": "SHOP",
                        "typeName": "РЎСѓРїРµСЂРјР°СЂРєРµС‚",
                        "weekSchedule": [
                            {
                                "dayNumber": 1,
                                "dayName": "РїРЅ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 2,
                                "dayName": "РІС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 3,
                                "dayName": "СЃСЂ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 4,
                                "dayName": "С‡С‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 5,
                                "dayName": "РїС‚",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 6,
                                "dayName": "СЃР±",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            },
                            {
                                "dayNumber": 7,
                                "dayName": "РІСЃ",
                                "workTime": {
                                    "workStartTime": "10:00",
                                    "workEndTime": "22:00"
                                }
                            }
                        ],
                        "geoPoint": {
                            "lat": 60.03383699999999834062691661529242992401123046875,
                            "lon": 30.367827999999999377678250311873853206634521484375
                        }
                    }
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "price",
            "title": "Р¦РµРЅР°",
            "displayType": "range_slider",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "from": 5369.0,
                    "to": 29235.0,
                    "min": 5369.0,
                    "max": 29235.0,
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom={facetValue}&f-priceto={facetValue}&f-prod_kind=prod_kind_botinki_i_sapogi"
                }
            ],
            "collapsedByDefault": false,
            "subqueryWoFacetVals": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-prod_kind=prod_kind_botinki_i_sapogi"
        },
        {
            "id": "prod_kind",
            "title": "РўРёРї С‚РѕРІР°СЂР°",
            "displayType": "list",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289",
                    "value": "prod_kind_botinki_i_sapogi",
                    "selectedByUser": true,
                    "isAvailable": true,
                    "uiCaption": "Р‘РѕС‚РёРЅРєРё Рё СЃР°РїРѕРіРё",
                    "subQueryColorModelCount": 11
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_butsy",
                    "value": "prod_kind_butsy",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "Р‘СѓС‚СЃС‹",
                    "subQueryColorModelCount": 41
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_krossovki_i_kedy",
                    "value": "prod_kind_krossovki_i_kedy",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "РљСЂРѕСЃСЃРѕРІРєРё Рё РєРµРґС‹",
                    "subQueryColorModelCount": 830
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_df3a20ec87",
                    "value": "prod_kind_df3a20ec87",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р‘СЂСЋРєРё Рё РїРѕР»СѓРєРѕРјР±РёРЅРµР·РѕРЅС‹",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_bryuki_futbolnye",
                    "value": "prod_kind_bryuki_futbolnye",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р‘СЂСЋРєРё С„СѓС‚Р±РѕР»СЊРЅС‹Рµ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_butylki_i_flyagi",
                    "value": "prod_kind_butylki_i_flyagi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р‘СѓС‚С‹Р»РєРё Рё С„Р»СЏРіРё",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_verkhnyaya_odezhda",
                    "value": "prod_kind_verkhnyaya_odezhda",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р’РµСЂС…РЅСЏСЏ РѕРґРµР¶РґР°",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_kovriki",
                    "value": "prod_kind_kovriki",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РљРѕРІСЂРёРєРё",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_myachi",
                    "value": "prod_kind_myachi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РњСЏС‡Рё",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_13a68375f1",
                    "value": "prod_kind_13a68375f1",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РќР°РїСѓР»СЊСЃРЅРёРєРё Рё РїРѕРІСЏР·РєРё",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_noski_i_getry",
                    "value": "prod_kind_noski_i_getry",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РќРѕСЃРєРё Рё РіРµС‚СЂС‹",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_87fdba89f7",
                    "value": "prod_kind_87fdba89f7",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РћР±СѓРІСЊ РґР»СЏ РµРґРёРЅРѕР±РѕСЂСЃС‚РІ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_sumki_i_chekhly",
                    "value": "prod_kind_sumki_i_chekhly",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЎСѓРјРєРё Рё С‡РµС…Р»С‹",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_taitsy_i_leginsy",
                    "value": "prod_kind_taitsy_i_leginsy",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РўР°Р№С‚СЃС‹ Рё Р»РµРіРёРЅСЃС‹",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_tolstovki_i_dzhempery",
                    "value": "prod_kind_tolstovki_i_dzhempery",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РўРѕР»СЃС‚РѕРІРєРё Рё РґР¶РµРјРїРµСЂС‹",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_utyazheliteli",
                    "value": "prod_kind_utyazheliteli",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЈС‚СЏР¶РµР»РёС‚РµР»Рё",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_forma_klubnaya",
                    "value": "prod_kind_forma_klubnaya",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р¤РѕСЂРјР° РєР»СѓР±РЅР°СЏ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_forma_futbolnaya",
                    "value": "prod_kind_forma_futbolnaya",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р¤РѕСЂРјР° С„СѓС‚Р±РѕР»СЊРЅР°СЏ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_futbolki_i_maiki",
                    "value": "prod_kind_futbolki_i_maiki",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р¤СѓС‚Р±РѕР»РєРё Рё РјР°Р№РєРё",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_sharfy_i_geitory",
                    "value": "prod_kind_sharfy_i_geitory",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЁР°СЂС„С‹ Рё РіРµР№С‚РѕСЂС‹",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_shlepantsy_i_sabo",
                    "value": "prod_kind_shlepantsy_i_sabo",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЁР»РµРїР°РЅС†С‹ Рё СЃР°Р±Рѕ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi,prod_kind_shorty_i_velosipedki",
                    "value": "prod_kind_shorty_i_velosipedki",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЁРѕСЂС‚С‹ Рё РІРµР»РѕСЃРёРїРµРґРєРё",
                    "subQueryColorModelCount": 0
                }
            ],
            "collapsedByDefault": false,
            "subqueryWoFacetVals": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289"
        },
        {
            "id": "size",
            "title": "Р Р°Р·РјРµСЂ",
            "displayType": "list",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_34d5",
                    "value": "size_34d5",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "34.5",
                    "subQueryColorModelCount": 1
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_35",
                    "value": "size_35",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "35",
                    "subQueryColorModelCount": 1
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_35d5",
                    "value": "size_35d5",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "35.5",
                    "subQueryColorModelCount": 2
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_36d5",
                    "value": "size_36d5",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "36.5",
                    "subQueryColorModelCount": 3
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_37",
                    "value": "size_37",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "37",
                    "subQueryColorModelCount": 1
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_37d5",
                    "value": "size_37d5",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "37.5",
                    "subQueryColorModelCount": 3
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_38",
                    "value": "size_38",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "38",
                    "subQueryColorModelCount": 5
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_39",
                    "value": "size_39",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "39",
                    "subQueryColorModelCount": 8
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_39d5",
                    "value": "size_39d5",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "39.5",
                    "subQueryColorModelCount": 8
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_40",
                    "value": "size_40",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "40",
                    "subQueryColorModelCount": 8
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_41",
                    "value": "size_41",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "41",
                    "subQueryColorModelCount": 7
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_41d5",
                    "value": "size_41d5",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "41.5",
                    "subQueryColorModelCount": 6
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_42",
                    "value": "size_42",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "42",
                    "subQueryColorModelCount": 4
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_43",
                    "value": "size_43",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "43",
                    "subQueryColorModelCount": 2
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_43d5",
                    "value": "size_43d5",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "43.5",
                    "subQueryColorModelCount": 1
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_5",
                    "value": "size_5",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "5",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_33-37",
                    "value": "size_33-37",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "33-37",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_34-38",
                    "value": "size_34-38",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "34-38",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_36",
                    "value": "size_36",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "36",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_37-41",
                    "value": "size_37-41",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "37-41",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_41-45",
                    "value": "size_41-45",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "41-45",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_42-44",
                    "value": "size_42-44",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "42-44",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_44",
                    "value": "size_44",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "44",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_44-46",
                    "value": "size_44-46",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "44-46",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_44d5",
                    "value": "size_44d5",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "44.5",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_45",
                    "value": "size_45",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "45",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_46",
                    "value": "size_46",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "46",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_46-48",
                    "value": "size_46-48",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "46-48",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_46d5",
                    "value": "size_46d5",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "46.5",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_47",
                    "value": "size_47",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "47",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_47d5",
                    "value": "size_47d5",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "47.5",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_48",
                    "value": "size_48",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "48",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_48d5",
                    "value": "size_48d5",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "48.5",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_50",
                    "value": "size_50",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "50",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_50-52",
                    "value": "size_50-52",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "50-52",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_52",
                    "value": "size_52",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "52",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_52-54",
                    "value": "size_52-54",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "52-54",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_54-56",
                    "value": "size_54-56",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "54-56",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_56-58",
                    "value": "size_56-58",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "56-58",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_60",
                    "value": "size_60",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "60",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-ra=size_bez_razmera",
                    "value": "size_bez_razmera",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р‘РµР· Р Р°Р·РјРµСЂР°",
                    "subQueryColorModelCount": 0
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "main_wcm_color",
            "title": "Р¦РІРµС‚",
            "displayType": "list",
            "displaySubType": "COLOR",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_korichnevyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_korichnevyi",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "РљРѕСЂРёС‡РЅРµРІС‹Р№",
                    "subQueryColorModelCount": 5,
                    "hex": "#964B00"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_sinii&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_sinii",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "РЎРёРЅРёР№",
                    "subQueryColorModelCount": 1,
                    "hex": "#2929FF"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_khaki&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_khaki",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "РҐР°РєРё",
                    "subQueryColorModelCount": 1,
                    "hex": "#7B917B"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_chernyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_chernyi",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "Р§РµСЂРЅС‹Р№",
                    "subQueryColorModelCount": 5,
                    "hex": "#000000"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_bezhevyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_bezhevyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р‘РµР¶РµРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#F2E8C9"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_belyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_belyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р‘РµР»С‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#FFFFFF"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_biryuzovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_biryuzovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р‘РёСЂСЋР·РѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#30D5C8"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_bordovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_bordovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р‘РѕСЂРґРѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#900020"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_vasilkovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_vasilkovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р’Р°СЃРёР»СЊРєРѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#6495ED"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_goluboi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_goluboi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р“РѕР»СѓР±РѕР№",
                    "subQueryColorModelCount": 0,
                    "hex": "#42AAFF"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_grafitovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_grafitovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р“СЂР°С„РёС‚РѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#585B62"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_zheltyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_zheltyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р–РµР»С‚С‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#FDE910"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_zelenyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_zelenyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р—РµР»РµРЅС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#009900"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_zolotoi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_zolotoi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р—РѕР»РѕС‚РѕР№",
                    "subQueryColorModelCount": 0,
                    "hex": "#FAD201"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_korallovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_korallovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РљРѕСЂР°Р»Р»РѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#FF7F50"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_krasnyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_krasnyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РљСЂР°СЃРЅС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#CC0000"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_laimovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_laimovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р›Р°Р№РјРѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#BFFF00"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_lilovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_lilovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р›РёР»РѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#DD4492"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_multitsvet&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_multitsvet",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РњСѓР»СЊС‚РёС†РІРµС‚",
                    "subQueryColorModelCount": 0,
                    "hex": "#FFFFFF"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_myatnyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_myatnyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РњСЏС‚РЅС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#30BA8F"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_oranzhevyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_oranzhevyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РћСЂР°РЅР¶РµРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#FF4F00"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_rozovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_rozovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р РѕР·РѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#F64A8A"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_salatovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_salatovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЎР°Р»Р°С‚РѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#7CFC00"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_serebristyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_serebristyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЎРµСЂРµР±СЂРёСЃС‚С‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#C0C0C0"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_seryi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_seryi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЎРµСЂС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#929292"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-main_wcm_color=main_color_fioletovyi&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "main_color_fioletovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р¤РёРѕР»РµС‚РѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "hex": "#8B00FF"
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "top_material",
            "title": "РњР°С‚РµСЂРёР°Р»",
            "displayType": "list",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_naturalnaya_kozha",
                    "value": "top_material_naturalnaya_kozha",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "РќР°С‚СѓСЂР°Р»СЊРЅР°СЏ РєРѕР¶Р°",
                    "subQueryColorModelCount": 2
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_4783b0574c",
                    "value": "top_material_4783b0574c",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "РЎРёРЅС‚РµС‚РёС‡РµСЃРєРёР№ РјР°С‚РµСЂРёР°Р»",
                    "subQueryColorModelCount": 9
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_iskusstvennaya_kozha",
                    "value": "top_material_iskusstvennaya_kozha",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РСЃРєСѓСЃСЃС‚РІРµРЅРЅР°СЏ РєРѕР¶Р°",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_naturalnaya_zamsha",
                    "value": "top_material_naturalnaya_zamsha",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РќР°С‚СѓСЂР°Р»СЊРЅР°СЏ Р·Р°РјС€Р°",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_naturalnyi",
                    "value": "top_material_naturalnyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РќР°С‚СѓСЂР°Р»СЊРЅС‹Р№",
                    "subQueryColorModelCount": 0,
                    "siteTip": "95% Рё Р±РѕР»РµРµ РІ СЃРѕСЃС‚Р°РІРµ РЅР°С‚СѓСЂР°Р»СЊРЅР°СЏ С‚РєР°РЅСЊ"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_naturalnyi_kauchuk",
                    "value": "top_material_naturalnyi_kauchuk",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РќР°С‚СѓСЂР°Р»СЊРЅС‹Р№ РєР°СѓС‡СѓРє",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_neilon",
                    "value": "top_material_neilon",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РќРµР№Р»РѕРЅ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_nubuk",
                    "value": "top_material_nubuk",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РќСѓР±СѓРє",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_plastik",
                    "value": "top_material_plastik",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РџР»Р°СЃС‚РёРє",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_poliuretan",
                    "value": "top_material_poliuretan",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РџРѕР»РёСѓСЂРµС‚Р°РЅ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_poliester",
                    "value": "top_material_poliester",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РџРѕР»РёСЌСЃС‚РµСЂ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_sinteticheskaya_kozha",
                    "value": "top_material_sinteticheskaya_kozha",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЎРёРЅС‚РµС‚РёС‡РµСЃРєР°СЏ РєРѕР¶Р°",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_sinteticheskii",
                    "value": "top_material_sinteticheskii",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЎРёРЅС‚РµС‚РёС‡РµСЃРєРёР№",
                    "subQueryColorModelCount": 0,
                    "siteTip": "РќР°С‚СѓСЂР°Р»СЊРЅР°СЏ С‚РєР°РЅСЊ РІ СЃРѕСЃС‚Р°РІРµ РѕС‚СЃСѓС‚СЃС‚РІСѓРµС‚"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_smesovyi",
                    "value": "top_material_smesovyi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РЎРјРµСЃРѕРІС‹Р№",
                    "subQueryColorModelCount": 0,
                    "siteTip": "1% - 95% РІ СЃРѕСЃС‚Р°РІРµ РЅР°С‚СѓСЂР°Р»СЊРЅР°СЏ С‚РєР°РЅСЊ"
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_tpe",
                    "value": "top_material_tpe",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РўРџР­",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_tekstil",
                    "value": "top_material_tekstil",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РўРµРєСЃС‚РёР»СЊ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_termopoliuretan",
                    "value": "top_material_termopoliuretan",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РўРµСЂРјРѕРїРѕР»РёСѓСЂРµС‚Р°РЅ",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_khlopok_s_sintetikoi",
                    "value": "top_material_khlopok_s_sintetikoi",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РҐР»РѕРїРѕРє СЃ СЃРёРЅС‚РµС‚РёРєРѕР№",
                    "subQueryColorModelCount": 0
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-top_material=top_material_eva",
                    "value": "top_material_eva",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "Р­Р’Рђ",
                    "subQueryColorModelCount": 0
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "moisture_protect",
            "title": "Р—Р°С‰РёС‚Р° РѕС‚ РІР»Р°РіРё",
            "displayType": "list",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-moisture_protect=moisture_protect_propitka&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "moisture_protect_propitka",
                    "selectedByUser": false,
                    "isAvailable": true,
                    "uiCaption": "РџСЂРѕРїРёС‚РєР°",
                    "subQueryColorModelCount": 2,
                    "siteTip": "РЎРїРµС†РёР°Р»РёР·РёСЂРѕРІР°РЅРЅР°СЏ РїСЂРѕРїРёС‚РєР°, РѕР±Р»Р°РґР°СЋС‰Р°СЏ РІРѕРґРѕРѕС‚С‚Р°Р»РєРёРІР°СЋС‰РёРјРё СЃРІРѕР№СЃС‚РІР°РјРё."
                },
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-moisture_protect=moisture_protect_otsutstvuet&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "moisture_protect_otsutstvuet",
                    "selectedByUser": false,
                    "isAvailable": false,
                    "uiCaption": "РћС‚СЃСѓС‚СЃС‚РІСѓРµС‚",
                    "subQueryColorModelCount": 0,
                    "siteTip": "Р—Р°С‰РёС‚Р° РѕС‚ РІР»Р°РіРё РѕС‚СЃСѓС‚СЃС‚РІСѓРµС‚."
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "except_sellers_user",
            "title": "РўРѕРІР°СЂС‹ РЎРїРѕСЂС‚РјР°СЃС‚РµСЂ",
            "displayType": "toggle",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-except_sellers_user=true&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "true",
                    "selectedByUser": false,
                    "isAvailable": true
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "sellers",
            "title": "РўРѕРІР°СЂС‹ РїСЂРѕРґР°РІС†РѕРІ",
            "displayType": "toggle",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi&f-sellers=true",
                    "value": "true",
                    "selectedByUser": false,
                    "isAvailable": true
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "is_foreign_product",
            "title": "РўРѕРІР°СЂС‹ РёР·-Р·Р° СЂСѓР±РµР¶Р°",
            "displayType": "toggle",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-is_foreign_product=is_foreign_product_da&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "false",
                    "selectedByUser": false,
                    "isAvailable": true
                }
            ],
            "collapsedByDefault": false
        },
        {
            "id": "premium",
            "title": "Premium",
            "displayType": "toggle",
            "displaySubType": "DEFAULT",
            "values": [
                {
                    "subqueryReference": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-premium=premium_da&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
                    "value": "false",
                    "selectedByUser": false,
                    "isAvailable": true
                }
            ],
            "collapsedByDefault": false,
            "siteTip": "РўРѕРІР°СЂ РїСЂРµРјРёСѓРј СЃРµРіРјРµРЅС‚Р°"
        }
    ],
    "subqueryRef": "/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi",
    "subqueryRefWoFacets": "/catalog/brendy/nike/for-man-all/"
}