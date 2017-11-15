using System;

namespace Requisition_Portal.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {

        public void Execute()
        {

         //   Mapper.CreateMap<DiscoModel_2, Disco>();

         //   Mapper.CreateMap<Disco, DiscoModel_2>();

         //   Mapper.CreateMap<DiscoMonthlySummary, DiscoSummaryModel>()
         //     .ForMember(d => d.Name, mp => { mp.MapFrom(x => x.DiscoName); });

         //   Mapper.CreateMap<DiscoSummaryModel, DiscoMonthlySummary>()
         //       .ForMember(d => d.DiscoName, mp => { mp.MapFrom(x => x.Name); });

         //   Mapper.CreateMap<ParticipantDistro, ParticipantSummaryModel>()
         //     .ForMember(d => d.DiscoId, mp => { mp.Ignore(); });

         //   Mapper.CreateMap<ParticipantSummaryModel, ParticipantDistro>()
         //       .ForMember(d => d.Name, mp => { mp.MapFrom(x => x.Name); });


         //   Mapper.CreateMap<DiscoModel, Disco>()
         //        .ForMember(d => d.BankId, mp => { mp.MapFrom(x => x.Bank.Id); });

         //   Mapper.CreateMap<Disco, DiscoModel>()
         //        .ForMember(d => d.Bank, mp => { mp.Ignore(); });

         //   Mapper.CreateMap<Genco, GencoModel>()
         //        .ForMember(d => d.Bank, mp => { mp.Ignore(); });

         //   Mapper.CreateMap<GencoModel, Genco>()
         //       .ForMember(d => d.TypeId, mp => { mp.MapFrom(x => x.GencoTypes.Id); })
         //       .ForMember(d => d.BankId, mp => { mp.MapFrom(x => x.Bank.Id); });



         //   Mapper.CreateMap<StationViewModel, GencoStation>()
         //        .ForMember(d => d.IsDeleted, mp => { mp.MapFrom(x => x.IsDeleted); })
         //         .ForMember(d => d.Name, mp => { mp.MapFrom(x => x.StationName); })

         //          .ForMember(d => d.GencoId, mp => { mp.MapFrom(x => x.GencoId); })
         //       //   .ForMember(d => d.Genco.Name, mp => { mp.MapFrom(x => x.GencoName); })

         //       .ForMember(d => d.Id, mp => { mp.MapFrom(x => x.StationId); });

         //   Mapper.CreateMap<StationViewModel, Genco>()
         // .ForMember(d => d.Name, mp => { mp.MapFrom(x => x.GencoName); })
         //.ForMember(d => d.Id, mp => { mp.MapFrom(x => x.GencoId); });

         //   Mapper.CreateMap<GencoDropDownModel, Genco>()
         //       .ForMember(d => d.Name, mp => { mp.MapFrom(x => x.Name); })
         //       .ForMember(d => d.Id, mp => { mp.MapFrom(x => x.Id); });

         //   Mapper.CreateMap<Genco, GencoDropDownModel>()
         //       .ForMember(d => d.Name, mp => { mp.MapFrom(x => x.Name); })
         //       .ForMember(d => d.Id, mp => { mp.MapFrom(x => x.Id); });

         //   Mapper.CreateMap<GencoTypeModel, GencoType>();



         //   Mapper.CreateMap<GencoType, GencoTypeModel>()
         //       .ForMember(d => d.Description, mp => { mp.MapFrom(x => x.Description); })
         //         .ForMember(d => d.Name, mp => { mp.MapFrom(x => x.Name); })
         //       .ForMember(d => d.Id, mp => { mp.MapFrom(x => x.Id); });

         //   Mapper.CreateMap<GencoStation, StationViewModel>()
         //       .ForMember(d => d.GencoName, mp => { mp.MapFrom(x => x.Genco.Name); })
         //       .ForMember(d => d.IsDeleted, mp => { mp.MapFrom(x => x.IsDeleted); })
         //       .ForMember(d => d.StationId, mp => { mp.MapFrom(x => x.Id); })
         //       .ForMember(d => d.StationName, mp => { mp.MapFrom(x => x.Name); })
         //       .ForMember(d => d.GencoId, mp => { mp.MapFrom(x => x.GencoId); })
         //       // .AfterMap();
         //       .ForMember(d => d.Gencos, mp => { mp.MapFrom(x => new Genco() { Id = x.Id, Name = x.Name }); });
         //   // .ForMember(d => d.Gencos, mp => { mp.MapFrom(x => Mapper.Map<GencoDropDownModel, Genco>(x.); });

         //   Mapper.CreateMap<UnitPriceModel, MonthlyUnitPrice>()
         //       .ForMember(d => d.CapacityUnitPrice, mp => { mp.MapFrom(x => x.CapacityUnitPrice); })
         //       .ForMember(d => d.EnergyUnitPrice, mp => { mp.MapFrom(x => x.EnergyUnitPrice); })
         //       .ForMember(d => d.Id, mp => { mp.MapFrom(x => x.Id); })
         //       //  .ForMember(d => d, mp => { mp.MapFrom(x => x.Station.Genco.GencoType); })
         //       .ForMember(d => d.MarketMonthId, mp => { mp.MapFrom(x => x.MonthId); })
         //       .ForMember(d => d.MarketYearId, mp => { mp.MapFrom(x => x.YearId); })
         //        .ForMember(d => d.StationId, mp => { mp.MapFrom(x => x.StationId); });

         //   Mapper.CreateMap<MonthlyUnitPrice, UnitPriceModel>()
         //       .ForMember(d => d.CapacityUnitPrice, mp => { mp.MapFrom(x => x.CapacityUnitPrice); })
         //      .ForMember(d => d.EnergyUnitPrice, mp => { mp.MapFrom(x => x.EnergyUnitPrice); })
         //      .ForMember(d => d.Id, mp => { mp.MapFrom(x => x.Id); })
         //      .ForMember(d => d.MonthId, mp => { mp.MapFrom(x => x.MarketMonthId); })
         //       .ForMember(d => d.GencoType, mp => { mp.MapFrom(x => x.Station.Genco.GencoType.Name); })
         //      .ForMember(d => d.YearId, mp => { mp.MapFrom(x => x.MarketYearId); })
         //       .ForMember(d => d.StationId, mp => { mp.MapFrom(x => x.StationId); });



         //   Mapper.CreateMap<GeneratorPriceCaptureModel, DiscoEnergyConsumption>()
         //     .ForMember(d => d.CapacitySharedInMW, mp => { mp.MapFrom(x => x.CapacityShared); })
         //      .ForMember(d => d.CapacityUnitPriceUsed, mp => { mp.MapFrom(x => x.CapacityUnitPrice); })
         //       .ForMember(d => d.DiscoId, mp => { mp.MapFrom(x => x.DiscoId); })
         //        .ForMember(d => d.EnergySharedInkWh, mp => { mp.MapFrom(x => x.EnergyShared); })
         //         .ForMember(d => d.EnergyUnitPriceUsed, mp => { mp.MapFrom(x => x.EnergyUnitPrice); })
         //          .ForMember(d => d.Id, mp => { mp.MapFrom(x => x.Id); })
         //           .ForMember(d => d.MarketMonthId, mp => { mp.MapFrom(x => x.MonthId); })
         //            .ForMember(d => d.MarketYearId, mp => { mp.MapFrom(x => x.YearId); })
         //             .ForMember(d => d.StationId, mp => { mp.MapFrom(x => x.StationId); })
         //              .ForMember(d => d.TotalCharge, mp => { mp.MapFrom(x => x.TotalCharge); })
         //               .ForMember(d => d.MonthlyUnitPriceId, mp => { mp.Ignore(); });

         //   Mapper.CreateMap<DiscoEnergyConsumption, GeneratorPriceCaptureModel>()
         //       .ForMember(d => d.CapacityShared, mp => { mp.MapFrom(x => x.CapacitySharedInMW); })
         //         .ForMember(d => d.CapacityUnitPrice, mp => { mp.MapFrom(x => x.CapacityUnitPriceUsed); })
         //        .ForMember(d => d.DiscoId, mp => { mp.MapFrom(x => x.DiscoId); })
         //         .ForMember(d => d.EnergyShared, mp => { mp.MapFrom(x => x.EnergySharedInkWh); })
         //          .ForMember(d => d.EnergyUnitPrice, mp => { mp.MapFrom(x => x.EnergyUnitPriceUsed); })
         //           .ForMember(d => d.Id, mp => { mp.MapFrom(x => x.Id); })
         //            .ForMember(d => d.MonthId, mp => { mp.MapFrom(x => x.MarketMonthId); })
         //             .ForMember(d => d.YearId, mp => { mp.MapFrom(x => x.MarketYearId); })
         //              .ForMember(d => d.StationId, mp => { mp.MapFrom(x => x.StationId); })
         //               .ForMember(d => d.TotalCharge, mp => { mp.MapFrom(x => x.TotalCharge); })
         //                .ForMember(d => d.TotalCapacityCharge, mp => { mp.MapFrom(x => x.CapacitySharedInMW * x.CapacityUnitPriceUsed); })
         //                   .ForMember(d => d.TotalEnergyCharge, mp => { mp.MapFrom(x => x.EnergyUnitPriceUsed * x.EnergySharedInkWh); })
         //               .ForMember(d => d.GencoType, mp => { mp.MapFrom(x => x.Station.Genco.GencoType.Name); });



         //   Mapper.CreateMap<ParticipantModel, Participant>()
         //          .ForMember(d => d.AccountNumber, mp => { mp.MapFrom(x => x.AccountNumber); })
         //             .ForMember(d => d.BankId, mp => { mp.MapFrom(x => x.Bank.Id); })
         //                .ForMember(d => d.MOCPercentage, mp => { mp.MapFrom(x => x.MOCPercentange); });


         //   Mapper.CreateMap<Bank, BankModel>();
         //   Mapper.CreateMap<BankModel, Bank>();


         //   Mapper.CreateMap<UnitOfMeasurement, UnitModel>();
         //   Mapper.CreateMap<UnitModel, UnitOfMeasurement>();

         //   Mapper.CreateMap<PaymentGroup, PaymentGroupModel>();
         //   Mapper.CreateMap<PaymentGroupModel, PaymentGroup>();

         //   Mapper.CreateMap<Participant, ParticipantModel>()
         //       .ForMember(d => d.MOCPercentange, mp => { mp.MapFrom(x => x.MOCPercentage); });


         //   Mapper.CreateMap<PaymentItemModel, PaymentItem>()
         //       .ForMember(d => d.IsDeleted, mp => { mp.MapFrom(x => x.IsDeleted); })
         //                              .ForMember(d => d.GroupId, mp => { mp.MapFrom(x => x.PaymentGroup.Id); })
         //                              .ForMember(d => d.Name, mp => { mp.MapFrom(x => x.Name); })
         //                              .ForMember(d => d.ParticipantId, mp => { mp.MapFrom(x => x.Participants.Id); })
         //                              .ForMember(d => d.UnitPrice, mp => { mp.MapFrom(x => x.UnitPrice); })
         //                              //  .ForMember(d => d.EntryDate, mp => { mp.MapFrom(x => DateTime.ParseExact(x.EntryDate,"dd/MM/yyyy", null )); })
         //                              .ForMember(d => d.UnitOfMeasureId, mp => { mp.MapFrom(x => x.Unit.Id); });

         //   Mapper.CreateMap<PaymentItem, PaymentItemModel>()
         //        .ForMember(d => d.UnitPrice, mp => { mp.MapFrom(x => x.UnitPrice); })

         //        .ForMember(d => d.Participants, mp => { mp.Ignore(); })
         //        .ForMember(d => d.PaymentGroup, mp => { mp.Ignore(); })
         //         .ForMember(d => d.Unit, mp => { mp.Ignore(); });


         //   Mapper.CreateMap<DiscoImbalance, DiscoImbalanceModel>()
         //        .ForMember(d => d.FromDiscoId, mp => { mp.MapFrom(x => x.DiscoId); })
         //        //   .ForMember(d => d.Discos, mp => { mp.MapFrom(x => x.DiscoId); })
         //        .ForMember(d => d.CaptionDate, mp => { mp.Ignore(); })
         //         .ForMember(d => d.Discos, mp => { mp.Ignore(); })
         //          .ForMember(d => d.Units, mp => { mp.Ignore(); });

         //   Mapper.CreateMap<DiscoImbalanceModel, DiscoImbalance>()
         //          .ForMember(d => d.Disco, mp => { mp.Ignore(); })
         //             .ForMember(d => d.Month, mp => { mp.Ignore(); })
         //                .ForMember(d => d.ToDisco, mp => { mp.Ignore(); }).
         //                ForMember(d => d.UnitId, mp => { mp.MapFrom(x => x.Units.Id); })
         //                .ForMember(d => d.ToDiscoId, mp => { mp.MapFrom(x => x.Discos.Id); })
         //                 .ForMember(d => d.DiscoId, mp => { mp.MapFrom(x => x.FromDiscoId); })
         //                   .ForMember(d => d.Unit, mp => { mp.Ignore(); })
         //                      .ForMember(d => d.Year, mp => { mp.Ignore(); });


        }

        public int Order()
        {
            throw new NotImplementedException();
        }
    }
}