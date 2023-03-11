import {
  animate,
  AUTO_STYLE,
  group,
  query,
  style,
  transition,
  trigger,
} from '@angular/animations';

export const FadeInAndOutRoute = trigger('routerTransition', [
  transition('* <=> *', [
    query(':enter, :leave', style({}), { optional: true }),
    group([
      query(
        ':enter',
        [
          style({ visibility: 'hidden', opacity: 0 }),
          animate(
            '0.5s ease-in-out',
            style({ visibility: AUTO_STYLE, opacity: 1 })
          ),
        ],
        { optional: true }
      ),
      query(
        ':leave',
        [
          style({ visibility: AUTO_STYLE, opacity: 1 }),
          animate(
            '0.5s ease-in-out',
            style({ visibility: 'hidden', opacity: 0 })
          ),
        ],
        { optional: true }
      ),
    ]),
  ]),
]);
