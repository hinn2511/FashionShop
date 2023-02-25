export class ChartParams {
    from = new Date();
    to = new Date();
    limit = 0;

    metric = "";

    constructor(from: Date, to: Date, limit?: number, metric?: string)
    {
        this.from = from;
        this.to = to;
        this.limit = limit;
        this.metric = metric;
    }
}

export class ChartData {
  name: string;
  value: number;
}

export class ChartMultipleData {
  name: string;
  series: ChartData[];
}

export class ChartFilter {
  from: Date;
  to: Date;
  name: string;

  description: string;

  constructor(from: Date, to: Date, name: string, description: string) {
    this.from = from;
    this.to = to;
    this.name = name;
    this.description = description;
  }
}

export function fnGetChartTimeFilterList() {
  return [
    new ChartFilter(getLastDate(7), getCurrentDate(), 'Last 7 days', `From ${getLastDate(7).toLocaleDateString()} to ${getCurrentDate().toLocaleDateString()}`),
    new ChartFilter(getLastMonth(), getCurrentDate(), 'Last month', `From ${getLastMonth().toLocaleDateString()} to ${getCurrentDate().toLocaleDateString()}`),
    new ChartFilter(getLastQuarter(), getCurrentDate(), 'Last quarter', `From ${getLastQuarter().toLocaleDateString()} to ${getCurrentDate().toLocaleDateString()}`),
    new ChartFilter(getLastYear(), getCurrentDate(), 'Last year', `From ${getLastYear().toLocaleDateString()} to ${getCurrentDate().toLocaleDateString()}`),
  ];
}

export function getCurrentDate() {
  return new Date();
}

export function getLastDate(length: number) {
  let currentDate = getCurrentDate();
  return new Date(currentDate.getTime() - length * 24 * 60 * 60 * 1000);
}

export function getLastMonth() {
  let currentDate = new Date();
  currentDate.setMonth(currentDate.getMonth() - 1);
  return currentDate;
}

export function getLastQuarter() {
  let currentDate = new Date();
  currentDate.setMonth(currentDate.getMonth() - 3);
  return currentDate;
}

export function getLastYear() {
  let currentDate = new Date();
  currentDate.setMonth(currentDate.getMonth() - 12);
  return currentDate;
}
