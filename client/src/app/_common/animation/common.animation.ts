import {
  animate,
  animateChild,
  query,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';

export const GrowAnimation = trigger('grow', [
  state('out', style({})),
  state(
    'in',
    style({
      height: '{{height}}',
      overflow: 'hidden',
    }),
    { params: { height: '100%' } }
  ),
  transition('out => in', animate('0.5s ease-in-out')),
  transition('in => out', animate('0.5s ease-in-out')),
]);

export const SlideLeftToRight = trigger('slideLeftToRight', [
  transition(':enter', [
    style({ transform: 'translateX(-100%)', opacity: 0, color: 'transparent' }),
    animate('500ms ease-in-out', style({ transform: 'translateX(0%)', opacity: 1 })),
  ]),
  transition(':leave', [
    animate('500ms ease-in-out', style({ transform: 'translateX(-100%)', opacity: 0, color: 'transparent' })),
  ]),
]);

export const SlideRightToLeft = trigger('slideRightToLeft', [
  transition(':enter', [
    style({ transform: 'translateX(100%)', opacity: 0, color: 'transparent' }),
    animate('500ms ease-in-out', style({ transform: 'translateX(0%)', opacity: 1 })),
  ]),
  transition(':leave', [
    animate('500ms ease-in-out', style({ transform: 'translateX(100%)', opacity: 0, color: 'transparent'})),
  ]),
]);

export const SlideTopToBottom = trigger('slideTopToBottom', [
  transition(':enter', [
    style({}),
    animate('500ms ease-in', style({ opacity: 1})),
  ]),
  transition(':leave', [
    animate('500ms ease-out', style({ height: '0px', 'z-index': 1, opacity: 0})),
  ]),
]);

export const SlideTopToBottom2 = trigger('slideTopToBottom2', [
  state('out', style({height: '100%',  opacity: 1})),
  state(
    'in',
    style({ height: '0px', 'z-index': 1, opacity: 0}),
  ),
  transition('out => in', animate('0.5s ease-in-out')),
  transition('in => out', animate('0.5s ease-in-out')),
]);


export const FadeInAndOut = trigger('fadeInAndOut', [
  transition(':enter', [
    style({ opacity: 0 }),
    animate(
      '500ms cubic-bezier(0.785, 0.135, 0.15, 0.86)',
      style({ opacity: 1 })
    ),
  ]),
  transition(':leave', [
    animate(
      '500ms cubic-bezier(0.785, 0.135, 0.15, 0.86)',
      style({ opacity: 0 })
    ),
  ]),
]);

const fadeIn = transition(':enter', [
  style({ opacity: 0 }), // start state
  animate(
    '{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)',
    style({ opacity: 1 })
  ),
]);

export const fadeOut = transition(':leave', [
  animate(
    '{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)',
    style({ opacity: 0 })
  ),
]);

export const fadeInAnimation = trigger('fadeIn', [fadeIn]);

export const fadeOutAnimation = trigger('fadeOut', [fadeOut]);
