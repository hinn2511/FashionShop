import {
  animate,
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


const fadeIn = transition(':enter', [
  style({ opacity: 0 }), // start state
  animate(
    "{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)",
    style({ opacity: 1 })
  )
]);

export const fadeOut = transition(':leave', [
  animate(
    "{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)",
    style({ opacity: 0 })
  )
]);

export const fadeInAnimation = trigger('fadeIn', [fadeIn]);

export const fadeOutAnimation = trigger('fadeOut', [fadeOut]);

