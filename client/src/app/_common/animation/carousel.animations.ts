import { style, animate, animation, keyframes, trigger, state, transition, group } from "@angular/animations";

// =========================
// Enum for referencing animations
// =========================
export enum AnimationType {
  Scale = "scale",
  Fade = "fade",
  Flip = "flip",
  JackInTheBox = "jackInTheBox"
}

// =========================
// Scale
// =========================
export const scaleIn = animation([
  style({ opacity: 0, transform: "scale(0.5)" }), // start state
  animate(
    "{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)",
    style({ opacity: 1, transform: "scale(1)" })
  )
]);

export const scaleOut = animation([
  animate(
    "{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)",
    style({ opacity: 0, transform: "scale(0.5)" })
  )
]);

// =========================
// Fade
// =========================
export const fadeIn = animation([
  style({ opacity: 0 }), // start state
  animate(
    "{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)",
    style({ opacity: 1 })
  )
]);

export const fadeOut = animation([
  animate(
    "{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)",
    style({ opacity: 0 })
  )
]);

// =========================
// Flip
// =========================
export const flipIn = animation([
  animate(
    "{{time}} cubic-bezier(0.785, 0.135, 0.15, 0.86)",
    keyframes([
      style({
        opacity: 1,
        transform: "perspective(400px) rotate3d(1, 0, 0, 90deg)",
        offset: 0
      }), // start state
      style({ transform: "perspective(400px)", offset: 1 })
    ])
  )
]);

export const flipOut = animation([
  // just hide it
]);

// =========================
// Jack in the box
// =========================
export const jackIn = animation([
  animate(
    "{{time}} ease-in",
    keyframes([
      style({
        animationFillMode: "forwards",
        transform: "scale(0.1) rotate(30deg)",
        transformOrigin: "center bottom",
        offset: 0
      }),
      style({
        transform: "rotate(-10deg)",
        offset: 0.5
      }),
      style({
        transform: "rotate(3deg)",
        offset: 0.7
      }),
      style({
        transform: "perspective(400px)",
        offset: 1
      })
    ])
  )
]);

export const jackOut = animation([
  // just hide it
]);


export const SlideInOutAnimation = [
  trigger('slideInOut', [
      state('in', style({
          'max-height': '100%', 'opacity': '1', 'visibility': 'visible'
      })),
      state('out', style({
          'max-height': '0px', 'opacity': '0', 'visibility': 'hidden'
      })),
      transition('in => out', [group([
          animate('400ms ease-in-out', style({
              'opacity': '0'
          })),
          animate('600ms ease-in-out', style({
              'max-height': '0px'
          })),
          animate('700ms ease-in-out', style({
              'visibility': 'hidden'
          }))
      ]
      )]),
      transition('out => in', [group([
          animate('1ms ease-in-out', style({
              'visibility': 'visible'
          })),
          animate('600ms ease-in-out', style({
              'max-height': '100%'
          })),
          animate('800ms ease-in-out', style({
              'opacity': '1'
          }))
      ]
      )])
  ]),
]

export const RotateAnimation = [
  trigger('rotatedState', [
    state('default', style({ transform: 'rotate(0)' })),
    state('rotated', style({ transform: 'rotate(-180deg)' })),
    transition('rotated => default', animate('500ms ease-out')),
    transition('default => rotated', animate('500ms ease-in')),
  ]),
];


export const slide = trigger("slide", [
  transition(":enter", [
    style({
      transform: "translateX({{startPos}}%)"
    }),
    animate(
      "500ms",
      style({
        transform: "translateX(0)",
        display: "flex"
      })
    )
  ]),
  transition(":leave", [
    style({
      transform: "translateX(0)",
      display: "flex"
    }),
    animate(
      "500ms",
      style({
        transform: "translateX({{endPos}}%)"
      })
    )
  ])
]);

export const slideInLeft = trigger("slideInLeft", [
  transition(":enter", [
    style({
      transform: "translateX(-100%)"
    }),
    animate(
      "500ms",
      style({
        transform: "translateX(0)",
        display: "flex"
      })
    )
  ]),
  transition(":leave", [
    style({
      transform: "translateX(0)",
      display: "flex"
    }),
    animate(
      "500ms",
      style({
        transform: "translateX(-100%)"
      })
    )
  ])
]);

export const slideInRight = trigger("slideInRight", [
  transition(":enter", [
    style({
      transform: "translateX(100%)"
    }),
    animate(
      "500ms",
      style({
        transform: "translateX(0)",
        display: "flex"
      })
    )
  ]),
  transition(":leave", [
    style({
      transform: "translateX(0)",
      display: "flex"
    }),
    animate(
      "500ms",
      style({
        transform: "translateX(100%)"
      })
    )
  ])
]);
