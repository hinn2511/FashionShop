import {
  animate,
  animateChild,
  AUTO_STYLE,
  query,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';

// Expand element from 0 to full height (boolean trigger)
export const ExpandHeight = trigger('expandHeight', [
  state('false', style({ height: '0px', visibility: 'hidden' })),
  state('true', style({ height: '{{fullHeight}}', visibility: AUTO_STYLE }),  { params: { fullHeight: '100%' } }),
  transition('false => true', animate('500ms ease-in')),
  transition('true => false', animate('500ms ease-out')),
]);

// Expand element from 0 to full width (boolean trigger)
export const ExpandWidth = trigger('expandWidth', [
  state('false', style({ width: '0px', visibility: 'hidden' })),
  state('true', style({ width: '{{fullWidth}}', visibility: AUTO_STYLE }),  { params: { fullWidth: '100%' } }),
  transition('false => true', animate('500ms ease-in')),
  transition('true => false', animate('500ms ease-out')),
]);

// Delete
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

// Slide from left of the screen to target position (ng-If trigger)
export const SlideLeftToRight = trigger('slideLeftToRight', [
  transition(':enter', [
    style({ transform: 'translateX(-100%)', opacity: 0, color: 'transparent'}),
    animate(
      '500ms ease-in-out',
      style({ transform: 'translateX(0%)', opacity: 1 })
    ),
  ]),
  transition(':leave', [
    animate(
      '500ms ease-in-out',
      style({
        transform: 'translateX(-100%)',
        opacity: 0,
        color: 'transparent'
      })
    ),
  ]),
]);

// Slide from left of the screen to target position (boolean trigger)
export const SlideLeftToRightBoolean = trigger('slideLeftToRightBoolean', [
  state('true', style({ transform: 'translateX(0%)', opacity: 1  })),
  state('false', style({ transform: 'translateX(-100%)', opacity: 0, color: 'transparent' })),
  transition('false => true', animate('500ms ease-in-out')),
  transition('true => false', animate('500ms ease-in-out')),
]);



// Slide from right of the screen to target position (ng-If trigger)
export const SlideRightToLeft = trigger('slideRightToLeft', [
  transition(':enter', [
    style({ transform: 'translateX(100%)', opacity: 0, color: 'transparent'}),
    animate(
      '500ms ease-in-out',
      style({ transform: 'translateX(0%)', opacity: 1 })
    ),
  ]),
  transition(':leave', [
    animate(
      '500ms ease-in-out',
      style({ transform: 'translateX(100%)',opacity: 0, color: 'transparent'})
    ),
  ]),
]);

// Expand from top of the screen to target position (ng-If trigger) // lagging
export const ExpandTopToBottom = trigger('expandTopToBottom', [
  transition(':enter', [
    style({ height: '0px' }),
    animate('500ms ease-in', style({ height: '100%' })),
    query('@fade', [
            animateChild()
          ])
  ]),
  transition(':leave', [animate('500ms ease-out', style({ height: '0px' }))]),
]);

// Slide from top of the screen to target position (ng-If trigger)
export const SlideTopToBottom = trigger('slideTopToBottom', [
  transition(':enter', [
    style({ transform: 'translateY(-100%)', opacity: 0, color: 'transparent' }),
    animate('500ms ease-in', style({ transform: 'translateY(0%)', opacity: 1})),
    query('@fade', [
            animateChild()
          ])
  ]),
  transition(':leave', [animate('500ms ease-out', style({ transform: 'translateY(-100%)', opacity: 0, color: 'transparent' }))]),
]);

export const Fade = trigger('fade', [
  transition(':enter', [
    style({ opacity: 0}),
    animate(
      '300ms ease-in',
      style({ opacity: 1 })
    ),
  ]),
  transition(':leave', [
    animate(
      '300ms ease-out',
      style({ opacity: 0})
    ),
  ]),
]);

// Slide from top of the screen to target position (value trigger)
export const SlideTopToBottomBoolean = trigger('SlideTopToBottomBoolean', [
  state('out', style({ height: '100%' })),
  state('in', style({ height: '0px' })),
  transition('out => in', animate('0.5s ease-in-out')),
  transition('in => out', animate('0.5s ease-in-out')),
]);


// Slowly visible with fade animation (ng-If trigger)
export const FadeInAndOut = trigger('fadeInAndOut', [
  transition(':enter', [
    style({ opacity: 0, visibility: 'hidden' }),
    animate(
      '500ms cubic-bezier(0.785, 0.135, 0.15, 0.86)',
      style({ opacity: 1 })
    ),
  ]),
  transition(':leave', [
    animate(
      '500ms cubic-bezier(0.785, 0.135, 0.15, 0.86)',
      style({ opacity: 0, visibility: 'hidden' })
    ),
  ]),
]);

export const SlideInOut =  trigger('slideInOut', [
  state(
    'in',
    style({
      transform: 'translate3d({{width}},0,0)',
    }),
    { params: { width: '-100%'}}
  ),
  state(
    'out',
    style({
      transform: 'translate3d(0, 0, 0)',
    })
  ),
  transition('in => out', animate('400ms ease-in-out')),
  transition('out => in', animate('400ms ease-in-out')),
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
