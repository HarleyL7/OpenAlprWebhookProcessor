import { Component, OnInit } from '@angular/core';
import { User } from '@app/_models';
import { AccountService } from '@app/_services';
import { HomeService } from './home.service';

@Component({ templateUrl: 'home.component.html' })
export class HomeComponent implements OnInit {
    user: User;
    public plateCounts: any[];

    view: any[] = [700, 400];
    constructor(
        private accountService: AccountService,
        private homeService: HomeService) {
        this.user = this.accountService.userValue;
    }
    
    ngOnInit() {
        this.homeService.getPlatesCount(7).subscribe(result => {
            this.plateCounts = result.weeks;
        });
    }

    name = 'Angular';
    showXAxis = true;
    showYAxis = true;
    gradient = false;
    showLegend = true;
    legendTitle = 'Legend';
    legendPosition = 'right';
    showXAxisLabel = true;
    tooltipDisabled = false;
    showText = true;
    xAxisLabel = 'Country';
    showYAxisLabel = true;
    yAxisLabel = 'GDP Per Capita';
    showGridLines = true;
    innerPadding = '10%';
    barPadding = 8;
    groupPadding = 16;
    roundDomains = false;
    maxRadius = 10;
    minRadius = 3;
    showSeriesOnHover = true;
    roundEdges: boolean = true;
    animations: boolean = true;
    xScaleMin: any;
    xScaleMax: any;
    yScaleMin: number;
    yScaleMax: number;
    showDataLabel = false;
    noBarWhenZero = true;
    trimXAxisTicks = true;
    trimYAxisTicks = true;
    rotateXAxisTicks = true;
    maxXAxisTickLength = 16;
    maxYAxisTickLength = 16;
    colorSets: any;
    colorScheme: any;
    schemeType: string = 'ordinal';
    selectedColorScheme: string;
      // heatmap
  heatmapMin: number = 0;
  heatmapMax: number = 12;
  calendarData: any[] = [];

  calendarTooltipText(c): string {
    return `
      <span class="tooltip-label">${c.label} • ${c.cell.date}</span>
      <span class="tooltip-val">${c.data.toLocaleString()}</span>
    `;
  }
}